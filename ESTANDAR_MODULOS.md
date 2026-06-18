# Estándar de módulos — guía de refactorización

Cómo dejar cada módulo (`UseCases/<Modulo>/`, su controller y, si hace falta, su repositorio)
consistente con el resto del proyecto. Basado en lo ya aplicado en **Auth, Carrito, CarritoItem y Categoria**.

> Patrón de referencia: mira `Carrito` (con ownership) o `Categoria` (catálogo) como ejemplos completos.

---

## Checklist rápido por módulo

- [ ] **Validaciones** FluentValidation para el input *con forma* (Create/Update) en `UseCases/<Modulo>/Validations/`.
- [ ] **Handlers** usan `ApiResponse`/`ApiPaginationResponse` con `ErrorType` correcto (no todo 404).
- [ ] **Create** devuelve `201` (`.Created`); listados vacíos devuelven `200` con `[]` (no 404).
- [ ] **Sin `GetAll()`-y-filtrar-en-memoria**: las existencias/consultas van a un repositorio específico.
- [ ] **Repositorio específico** (si aplica) expuesto como propiedad del `IUnitOfWork`.
- [ ] **Controller** delgado: `[Authorize]` de clase + roles por endpoint; `ApiControllerBase` solo si usa `CurrentUser`.
- [ ] **Ownership** (si aplica) por `SolicitanteId` (token) + chequeo en el handler → `403`.
- [ ] Sin `using`/alias sobrantes; mensajes sin texto corrupto.

---

## 1. Use Cases (handlers)

Cada operación de negocio = un Command/Query (el command **es** el request DTO; en `Domain` solo van response DTOs).

### Respuesta y status codes
Los handlers retornan `ApiResponse<T>` / `ApiPaginationResponse<T>` etiquetando el fallo con `ErrorType`,
y `ToActionResult` lo traduce a HTTP:

| Situación | Factory | HTTP |
|---|---|---|
| Lectura/Update OK con datos | `Ok(data)` | 200 |
| Creación OK | `Created(data)` | 201 |
| Input *con forma* inválido | (lo lanza FluentValidation) | 400 |
| Regla de negocio sobre input | `Fail(msg, ErrorType.Validation)` | 400 |
| Credenciales ausentes/incorrectas | `Fail(msg, ErrorType.Unauthorized)` | 401 |
| Autenticado pero sin permiso / no es su recurso | `Fail(msg, ErrorType.Forbidden)` | 403 |
| Recurso no existe | `Fail(msg, ErrorType.NotFound)` | 404 |
| Choca con estado existente (duplicado, tiene asociados) | `Fail(msg, ErrorType.Conflict)` | 409 |

- `Fail(...)` sin `ErrorType` → **404 por defecto** (retro-compatible). Etiqueta siempre el motivo real.
- **Listado vacío NO es error**: nunca `Fail("no se encontraron...")`. Devuelve `Ok([...])` (200 con `data: []`).

### Qué va en el handler y qué NO
- **SÍ**: reglas que dependen de la BD (existe / es el dueño / duplicado / tiene asociados), orquestación, mapeo a DTO, `_uow.Complete()`.
- **NO**: validación de *forma* del input (eso es FluentValidation, ver §2).
- **NO**: llamar a otro use case (`_mediator.Send` dentro de un handler). El reuso va **hacia abajo**, a repositorios/domain services — no de un use case a otro.

---

## 2. Validaciones (FluentValidation)

- Carpeta: `UseCases/<Modulo>/Validations/`. Un validador por command/query **con input real**.
- Solo donde haya input *del cliente* que amerite rechazo: Create/Update, y filtros de query. Los ids de ruta/token
  o campos triviales no necesitan validador propio.
- Se **auto-registran** (`AddValidatorsFromAssembly` escanea el assembly) — no hay que registrarlos a mano.
- Corren **antes** del handler (vía `ValidationPipelineBehavior`); si fallan lanzan `ValidationException` → **400**.

```csharp
public class CreateXxxCommandValidator : AbstractValidator<CreateXxxCommand>
{
    public CreateXxxCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(255).WithMessage("...");   // que coincida con la columna
        RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("...");
    }
}
```

- Mover al validador los chequeos de forma que estaban en el handler (`if (cantidad <= 0) Fail(...)`, `IsNullOrWhiteSpace`, etc.).
- Lo que es **normalización** (ej. `Trim()`) se queda en el handler.
- Paginación: **no** se valida — se normaliza/acota (`Math.Clamp` en `GetPaged`) o simplemente la página vacía devuelve `[]`.

---

## 3. Repositorios específicos

### ¿Cuándo crear uno?
Crea `I<Entidad>Repository` cuando el módulo necesita **consultas que el genérico no cubre**, típicamente:
- Existencias/unicidad: reemplazar `(_uow.Repository<T>().GetAll()).Any(x => ...)` por una consulta filtrada.
- Búsquedas por campo: `GetByEmail`, etc.
- "¿tiene asociados?" para borrados: `TieneItems`, `TienePrendas`, ...

Si el módulo solo hace CRUD por id + listado paginado, **no necesita repo propio** → usa `_uow.Repository<T>()`.

### El patrón
La interfaz hereda del genérico y añade solo lo propio:

```csharp
// Domain/Ports/IRepositories/IXxxRepository.cs
public interface IXxxRepository : IGenericRepository<Xxx>
{
    Task<bool> ExisteConNombre(string nombre, long? exceptoId = null);
    Task<bool> TieneAsociados(long id);
}
```

La implementación hereda `GenericRepository<Xxx>` (trae CRUD gratis) y añade las consultas con EF
(se traducen a SQL eficiente: `SELECT EXISTS`, `WHERE`, etc.):

```csharp
// Infrastructure/Adapters/Repositories/XxxRepository.cs
public class XxxRepository(MixAndMatchDbContext context)
    : GenericRepository<Xxx>(context), IXxxRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConNombre(string nombre, long? exceptoId = null) =>
        _context.Set<Xxx>().AnyAsync(x => x.Nombre == nombre && (exceptoId == null || x.Id != exceptoId));

    public Task<bool> TieneAsociados(long id) =>
        _context.Set<OtraEntidad>().AnyAsync(o => o.XxxId == id);   // puede consultar tablas hijas
}
```

### Qué NO poner en el repositorio
- **Paginación específica**: `GetPaged(page, pageSize)` ya está en el genérico — sirve para cualquier entidad.
- **Lógica de presentación** (status HTTP, DTOs): eso es de Application/Api.
- Reglas de negocio compartidas → **domain service**, no en el repo ni en otro use case.

---

## 4. Registrar en el UnitOfWork (no en DI)

Los repos específicos los **expone el `UnitOfWork`** como propiedades *lazy* que comparten su `DbContext`.
**No** se registran en `InfrastructureServicesExtensions` (solo se registra `IUnitOfWork`).

```csharp
// Domain/Ports/IRepositories/IUnitOfWork.cs
IXxxRepository Xxx { get; }

// Infrastructure/Adapters/Repositories/UnitOfWork.cs
private IXxxRepository? _xxx;
public IXxxRepository Xxx => _xxx ??= new XxxRepository(_context);
```

En los handlers:
```csharp
public class CreateXxxCommandHandler(IUnitOfWork _uow) : IRequestHandler<...>
{
    // consultas/staging por el repo + commit por el UoW
    if (await _uow.Xxx.ExisteConNombre(nombre)) return Fail(..., ErrorType.Conflict);
    await _uow.Xxx.Add(entity);
    await _uow.Complete();
}
```

- Entidades **sin** repo propio se siguen usando con `_uow.Repository<T>()`.
- Un repo nuevo = 2 ediciones (propiedad en la interfaz + en el `UnitOfWork`), **sin tocar DI**.

---

## 5. Controllers

Delgados: arman el command/query, `_mediator.Send`, `this.ToActionResult(...)`.

> **Siempre `this.ToActionResult(result)`, nunca `return Ok(result)`.** `Ok(...)` envuelve todo en **200**,
> así que un `Fail` saldría como `200` con `success:false` y el cliente nunca vería el 4xx. `ToActionResult`
> es quien traduce el `ErrorType`/`SuccessType` al status real.

### Autorización
- `[Authorize]` **a nivel de clase** (deny-by-default; cubre endpoints futuros sin auth).
- Roles **por endpoint** con `[Authorize(Roles = nameof(RolUsuario.X))]` o `$"{nameof(...)},{nameof(...)}"` (coma = OR).
- Endpoints públicos: `[AllowAnonymous]`.
- Catálogo típico: lectura `ADMIN,CLIENTE`; escritura `ADMIN`.

### CurrentUser y ownership
- Hereda **`ApiControllerBase`** solo si el módulo necesita `CurrentUser` (ownership). Si es catálogo global
  (Categoria, Marca…), `ControllerBase` + `[ApiController]` está bien.
- **`[ApiController]`**: `ApiControllerBase` **ya lo lleva** encima — si heredas de ella **no** lo repitas.
  Solo se pone cuando heredas `ControllerBase` directo (catálogos). Por eso `DatosEnvioController : ApiControllerBase`
  no tiene `[ApiController]` y `CategoriasController : ControllerBase` sí.
- Para "solo el dueño toca su recurso": el controller pasa `CurrentUser.Id` como `SolicitanteId`,
  y el **handler** valida la pertenencia → `ErrorType.Forbidden`.
- Campos que pone el controller (desde token o ruta) van con **`[JsonIgnore]`** en el command para que el
  cliente no los pueda falsificar:

```csharp
public class UpdateXxxCommand : IRequest<...>
{
    [JsonIgnore] public long XxxId { get; set; }          // de la ruta
    public required string Campo { get; set; }            // del body
    [JsonIgnore] public long SolicitanteId { get; set; }  // del token
}
```

```csharp
[HttpPut("{id}")]
[Authorize(Roles = nameof(RolUsuario.CLIENTE))]
public async Task<IActionResult> Update(long id, [FromBody] UpdateXxxCommand command)
{
    command.XxxId = id;
    command.SolicitanteId = CurrentUser.Id;
    return this.ToActionResult(await _mediator.Send(command));
}
```

Ownership indirecto (ej. un item cuyo dueño es el carrito): el handler carga el recurso, carga su padre
(`_uow.Carritos.GetById(item.CarritoId)`) y compara `UsuarioId` → `Forbidden`.

---

## 6. Principios de diseño (el porqué)

1. **Un use case = una intención de negocio**, no cada consulta mínima. "¿existe X?" es un sub-paso → repo,
   no un use case.
2. **El use case no se ata 1:1 al endpoint**: HTTP es el driver más común, pero un job/cola/CLI podría disparar
   el mismo use case. Hoy coinciden porque el único driver es HTTP.
3. **El reuso fluye hacia abajo**: los use cases comparten **puertos** (repositorios) y **domain services**;
   nunca se llaman entre sí (`_mediator.Send` dentro de un handler = anti-patrón).
4. **La BD es la fuente de verdad** para unicidad: el chequeo `Existe...` es UX; el guardián real es la
   constraint `UNIQUE` (ver `UNIQUE_CONSTRAINT_STRATEGY.md`).
5. **Cada capa traduce en su frontera**: input mal formado → FluentValidation (400); regla de dominio →
   `ApiResponse.Fail(ErrorType)`; error técnico de BD → se mapea en el `GlobalExceptionHandler`.

---

## Anti-patrones a evitar

| ❌ Anti-patrón | ✅ En su lugar |
|---|---|
| `(_uow.Repository<T>().GetAll()).Any(x => ...)` | método de existencia en el repo (`AnyAsync` filtrado) |
| `Fail("...")` para todo (sale 404) | etiquetar con `ErrorType` (401/403/404/409) |
| Lista vacía → `Fail` (404) | `Ok([...])` (200 con `data: []`) |
| `return Ok(result)` en el controller (todo sale 200, oculta los fallos) | `return this.ToActionResult(result)` |
| Repetir `[ApiController]` al heredar `ApiControllerBase` | omitirlo (ya lo trae la base) |
| Validar `cantidad <= 0` en el handler | regla en el validador (400) |
| `_mediator.Send(otraQuery)` dentro de un handler | llamar al repositorio/domain service |
| Registrar cada repo en DI | exponerlo como propiedad del `UnitOfWork` |
| Use case por cada operación mínima | use cases gruesos; lo fino, en puertos |
