# Guía: implementar un módulo / endpoint nuevo (paso a paso)

Receta **en orden de construcción**, de cero a endpoint funcionando, siguiendo la arquitectura
hexagonal + CQRS/MediatR del proyecto. Las *reglas* (status codes, anti-patrones, ownership) están en
[`ESTANDAR_MODULOS.md`](ESTANDAR_MODULOS.md); **esta guía es el "cómo, en qué orden y dónde"**.

> Ejemplo guía de este documento: módulo **`Favorito`** (un usuario marca prendas como favoritas).
> Es un módulo *con dueño* + `UNIQUE(usuario_id, prenda_id)` + validación de FK, así que ejerce
> todas las piezas. Donde un **catálogo** (tipo `Categoria`) difiera, lo marco con 🏷️.

---

## Paso 0 — Decidir antes de escribir código

Responde estas 3 preguntas; definen casi todo lo demás:

| Pregunta | Opciones | Consecuencia |
|---|---|---|
| ¿El recurso **tiene dueño**? | Catálogo global / Propio del usuario | Si es propio → `SolicitanteId` del token + chequeo de pertenencia → `403`. |
| ¿Necesita **repo propio**? | Sí / No | Sí si hay unicidad, búsqueda por campo o "¿tiene asociados?". Si solo es CRUD por id + listado → `_uow.Repository<T>()`. |
| ¿Quién puede **leer / escribir**? | por rol | Catálogo: leer `ADMIN,CLIENTE`, escribir `ADMIN`. Propio: el `CLIENTE` dueño + `ADMIN`. |

**Decisión para el ejemplo `Favorito`:** tiene dueño (CLIENTE), necesita repo propio
(`ExisteFavorito` por el `UNIQUE`), y lo gestiona el CLIENTE dueño.

---

## Mapa de archivos a tocar

```
docker/postgres/init.sql                                  (0) tabla + UNIQUE + FKs
MixAndMatch.Domain/
  Entities/Favorito.cs                                    (1) entidad EF
  DTOs/Favoritos/FavoritoResponseDto.cs                   (2) DTO de respuesta
  Ports/IRepositories/IFavoritoRepository.cs              (3a) interfaz del repo
  Ports/IRepositories/IUnitOfWork.cs                      (4)  + propiedad
MixAndMatch.Infrastructure/
  Adapters/Repositories/FavoritoRepository.cs             (3b) impl del repo
  Adapters/Repositories/UnitOfWork.cs                     (4)  + propiedad lazy
MixAndMatch.Application/UseCases/Favorito/
  Commands/CreateFavoritoCommand.cs                       (5) handler + command
  Commands/DeleteFavoritoCommand.cs                       (5)
  Queries/GetAllFavoritosQuery.cs                         (5)
  Queries/GetFavoritoByIdQuery.cs                         (5)
  Validations/CreateFavoritoCommandValidator.cs           (6)
MixAndMatch.Api/Controllers/FavoritosController.cs        (7) endpoints
```

> Orden recomendado: **de adentro hacia afuera** (BD → dominio → infraestructura → aplicación → API).
> Cada capa solo depende de la de adentro, así compilas incrementalmente sin referencias rotas.

---

## Paso 1 — Esquema en `init.sql`

La BD es la fuente de verdad (longitudes, `UNIQUE`, FKs). Define la tabla **primero**: de aquí salen las
longitudes de los validadores y las constraints que justifican el repo.

```sql
CREATE TABLE IF NOT EXISTS favorito (
    id         BIGSERIAL   PRIMARY KEY NOT NULL,
    usuario_id BIGINT      NOT NULL REFERENCES usuarios(id),
    prenda_id  BIGINT      NOT NULL REFERENCES prenda(id),
    created_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ,
    UNIQUE (usuario_id, prenda_id)   -- un usuario no repite la misma prenda
);
```

> Recrear la BD: `docker compose down -v` y levantar de nuevo (los `TIMESTAMPTZ` y enums se crean al init).

---

## Paso 2 — Entidad (`Domain/Entities`)

POCO que mapea la tabla. Tipos: `long` para ids/FK, `decimal` para dinero, enums CLR para los enums nativos,
`DateTime?` para timestamps. Sin lógica de presentación.

```csharp
// MixAndMatch.Domain/Entities/Favorito.cs
namespace MixAndMatch.Domain.Entities;

public partial class Favorito
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }   // ← dueño: clave del ownership
    public long PrendaId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Prenda Prenda { get; set; } = null!;
}
```

> Si agregas una entidad nueva, súmala al `DbSet`/configuración del `MixAndMatchDbContext` como las demás.

---

## Paso 3 — DTO de respuesta (`Domain/DTOs`)

En `Domain` **solo van DTOs de respuesta** (el Command **es** el request DTO; no crees `*RequestDto`).
Nunca expongas campos sensibles (p. ej. `Contrasenia`). Los enums se exponen como `string` con `.ToString()`.

```csharp
// MixAndMatch.Domain/DTOs/Favoritos/FavoritoResponseDto.cs
namespace MixAndMatch.Domain.DTOs.Favoritos;

public class FavoritoResponseDto
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }
    public long PrendaId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

---

## Paso 3b — Repositorio específico (solo si aplica)

Crea `I<Entidad>Repository` cuando necesites consultas que el genérico no da. Para `Favorito`: el chequeo
de unicidad del `UNIQUE`. La interfaz hereda del genérico y añade **solo lo propio**:

```csharp
// MixAndMatch.Domain/Ports/IRepositories/IFavoritoRepository.cs
using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IFavoritoRepository : IGenericRepository<Favorito>
{
    Task<bool> ExisteFavorito(long usuarioId, long prendaId, long? exceptoId = null);
}
```

La implementación hereda `GenericRepository<Favorito>` (CRUD gratis) y añade la consulta con EF
(se traduce a `SELECT EXISTS`, sin traer filas a memoria):

```csharp
// MixAndMatch.Infrastructure/Adapters/Repositories/FavoritoRepository.cs
using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class FavoritoRepository(MixAndMatchDbContext context)
    : GenericRepository<Favorito>(context), IFavoritoRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteFavorito(long usuarioId, long prendaId, long? exceptoId = null) =>
        _context.Set<Favorito>()
            .AnyAsync(f => f.UsuarioId == usuarioId && f.PrendaId == prendaId
                           && (exceptoId == null || f.Id != exceptoId));
}
```

> 🏷️ Catálogo sin unicidad/relaciones: **omite este paso** y usa `_uow.Repository<T>()` en el handler.
> Recuerda: `GetPaged` ya está en el genérico — **no** lo dupliques en el repo específico.

---

## Paso 4 — Exponerlo en el `UnitOfWork` (NO en DI)

Los repos específicos se exponen como **propiedades lazy** del `UnitOfWork` que comparten su `DbContext`.
Dos ediciones, **sin tocar `InfrastructureServicesExtensions`**:

```csharp
// MixAndMatch.Domain/Ports/IRepositories/IUnitOfWork.cs   (dentro de la interfaz)
IFavoritoRepository Favoritos { get; }
```

```csharp
// MixAndMatch.Infrastructure/Adapters/Repositories/UnitOfWork.cs
private IFavoritoRepository? _favoritos;
public IFavoritoRepository Favoritos => _favoritos ??= new FavoritoRepository(_context);
```

---

## Paso 5 — Use Cases (Commands / Queries)

Una intención de negocio = un Command/Query. El handler hace: reglas que dependen de la BD
(existe / es el dueño / duplicado), mapeo a DTO y `_uow.Complete()`. **No** valida la *forma* del input
(eso es el validador) ni llama a otro use case.

### Create (FK → 400, dueño desde token, duplicado → 409, devuelve 201)

```csharp
// UseCases/Favorito/Commands/CreateFavoritoCommand.cs
using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Favoritos;
using MixAndMatch.Domain.Ports.IRepositories;
using FavoritoEntity = MixAndMatch.Domain.Entities.Favorito;

namespace MixAndMatch.Application.UseCases.Favorito.Commands;

public class CreateFavoritoCommand : IRequest<ApiResponse<FavoritoResponseDto>>
{
    public required long PrendaId { get; set; }

    [JsonIgnore]   // lo pone el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }
}

public class CreateFavoritoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateFavoritoCommand, ApiResponse<FavoritoResponseDto>>
{
    public async Task<ApiResponse<FavoritoResponseDto>> Handle(CreateFavoritoCommand request, CancellationToken ct)
    {
        // FK existe → si no, es input inválido (400), no 404.
        if (await _uow.Prendas.GetById(request.PrendaId) is null)
            return ApiResponse<FavoritoResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.", ErrorType.Validation);

        // Unicidad (respaldada por el UNIQUE) → 409.
        if (await _uow.Favoritos.ExisteFavorito(request.SolicitanteId, request.PrendaId))
            return ApiResponse<FavoritoResponseDto>.Fail("La prenda ya está en tus favoritos.", ErrorType.Conflict);

        var entity = new FavoritoEntity
        {
            UsuarioId = request.SolicitanteId,
            PrendaId = request.PrendaId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Favoritos.Add(entity);
        await _uow.Complete();

        return ApiResponse<FavoritoResponseDto>.Created(new FavoritoResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            PrendaId = entity.PrendaId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
```

### Delete (hoja: not-found 404 + chequeo de dueño → 403)

```csharp
// UseCases/Favorito/Commands/DeleteFavoritoCommand.cs
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Favorito.Commands;

public class DeleteFavoritoCommand : IRequest<ApiResponse<bool>>
{
    public required long FavoritoId { get; set; }
    public required long SolicitanteId { get; set; }
    public required bool EsAdmin { get; set; }
}

public class DeleteFavoritoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteFavoritoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteFavoritoCommand request, CancellationToken ct)
    {
        var entity = await _uow.Favoritos.GetById(request.FavoritoId);
        if (entity is null)
            return ApiResponse<bool>.Fail($"Favorito no encontrado para id {request.FavoritoId}.");

        if (!request.EsAdmin && entity.UsuarioId != request.SolicitanteId)
            return ApiResponse<bool>.Fail("No tienes acceso a este favorito.", ErrorType.Forbidden);

        await _uow.Favoritos.Delete(request.FavoritoId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Favorito eliminado correctamente.");
    }
}
```

### GetById (ownership directo) y GetAll (lista vacía = 200 `[]`)

```csharp
// UseCases/Favorito/Queries/GetFavoritoByIdQuery.cs   (resumen)
var entity = await _uow.Favoritos.GetById(request.FavoritoId);
if (entity is null)
    return ApiResponse<FavoritoResponseDto>.Fail($"Favorito no encontrado para id {request.FavoritoId}.");
if (!request.EsAdmin && entity.UsuarioId != request.SolicitanteId)
    return ApiResponse<FavoritoResponseDto>.Fail("No tienes acceso a este favorito.", ErrorType.Forbidden);
return ApiResponse<FavoritoResponseDto>.Ok(/* map a DTO */);
```

```csharp
// UseCases/Favorito/Queries/GetAllFavoritosQuery.cs   (resumen)
var (items, total) = await _uow.Favoritos.GetPaged(request.Page, request.PageSize);
// Lista vacía NO es error: 200 con data: [].
return ApiPaginationResponse<FavoritoResponseDto>.Ok(
    items.Select(x => new FavoritoResponseDto { /* map */ }), total, request.Page, request.PageSize);
```

> **Nunca** `if (!items.Any()) Fail(...)`. **Nunca** `_mediator.Send(...)` dentro de un handler.

---

## Paso 6 — Validadores (FluentValidation)

Carpeta `UseCases/<Modulo>/Validations/`. Solo para input **con forma** del cliente (Create/Update).
Se **auto-registran** y corren antes del handler; si fallan → **400**. Los ids de ruta/token no llevan validador.

```csharp
// UseCases/Favorito/Validations/CreateFavoritoCommandValidator.cs
using FluentValidation;
using MixAndMatch.Application.UseCases.Favorito.Commands;

namespace MixAndMatch.Application.UseCases.Favorito.Validations;

public class CreateFavoritoCommandValidator : AbstractValidator<CreateFavoritoCommand>
{
    public CreateFavoritoCommandValidator()
    {
        RuleFor(x => x.PrendaId)
            .GreaterThan(0).WithMessage("El id de la prenda debe ser mayor que cero.");
    }
}
```

Reglas de oro:
- **Forma** (longitud, `> 0`, `NotEmpty`, formato de enum) → validador.
- **Normalización** (`Trim()`) → se queda en el handler.
- **Paginación NO se valida** → se acota (`Math.Clamp`) o la página vacía devuelve `[]`.
- Longitudes (`MaximumLength`) **deben coincidir** con la columna de `init.sql`.

---

## Paso 7 — Controller

Delgado: arma el command/query, `_mediator.Send`, y **siempre** `this.ToActionResult(...)`
(nunca `return Ok(result)`, que aplastaría todo a 200 ocultando los 4xx).

```csharp
// MixAndMatch.Api/Controllers/FavoritosController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Favorito.Commands;
using MixAndMatch.Application.UseCases.Favorito.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]                                   // deny-by-default a nivel de clase
public class FavoritosController(IMediator _mediator) : ApiControllerBase   // hereda CurrentUser; NO repetir [ApiController]
{
    [HttpGet]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _mediator.Send(new GetAllFavoritosQuery { Page = page, PageSize = pageSize }));

    [HttpGet("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]   // coma = OR
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetFavoritoByIdQuery
        {
            FavoritoId = id,
            SolicitanteId = CurrentUser.Id,
            EsAdmin = CurrentUser.IsAdmin
        }));

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Create([FromBody] CreateFavoritoCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;                 // token, no body
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteFavoritoCommand
        {
            FavoritoId = id,
            SolicitanteId = CurrentUser.Id,
            EsAdmin = CurrentUser.IsAdmin
        }));
}
```

> 🏷️ Catálogo global (sin `CurrentUser`): hereda `ControllerBase` y **sí** pon `[ApiController]`; lectura
> `ADMIN,CLIENTE`, escritura `ADMIN`. Ej: `CategoriasController`.

---

## Paso 8 — Compilar y verificar

```bash
dotnet build MixAndMatch.Infrastructure/MixAndMatch.Infrastructure.csproj --no-incremental -clp:ErrorsOnly
dotnet build MixAndMatch.Application/MixAndMatch.Application.csproj   --no-incremental -clp:ErrorsOnly
```

- Errores `MSB3027` / `MSB3021` = **lock de archivos** porque la API está corriendo; **no** son errores de C#.
  Detén la API y reconstruye.
- Si tocaste `init.sql`, recrea la BD (`docker compose down -v`) antes de probar.

---

## Definition of Done (checklist final)

- [ ] Tabla en `init.sql` con longitudes, FKs y `UNIQUE` correctos.
- [ ] Entidad + DTO de respuesta (sin campos sensibles; enums como `string`).
- [ ] Repo específico **solo si** hay unicidad/búsqueda/asociados; expuesto en el `UnitOfWork` (no en DI).
- [ ] Handlers con `ErrorType` correcto: `201` al crear, `200 []` en listas vacías, `400/403/404/409` según caso.
- [ ] Sin `GetAll()`-y-filtrar-en-memoria; sin `_mediator.Send` dentro de un handler.
- [ ] Validadores para input con forma; longitudes = columnas.
- [ ] Controller con `[Authorize]` de clase + roles por endpoint; `this.ToActionResult(...)`; `[JsonIgnore]` en campos de token/ruta.
- [ ] Ownership (si aplica) validado en el handler → `403`.
- [ ] `using`/alias limpios; sin texto corrupto; **build en verde**.

---

### Tabla de decisión rápida de roles

| Tipo de módulo | GetAll | GetById | Create | Update | Delete |
|---|---|---|---|---|---|
| 🏷️ Catálogo (Categoria, Marca, Talla…) | `ADMIN,CLIENTE` | `ADMIN,CLIENTE` | `ADMIN` | `ADMIN` | `ADMIN` |
| 👤 Propio del cliente (Favorito, CarritoItem…) | `ADMIN` | `ADMIN` o dueño | `CLIENTE` | `CLIENTE` o dueño | `CLIENTE` o dueño |
| ⚙️ Operativo (Envio…) | `ADMIN` | `ADMIN` | `ADMIN` | `ADMIN` | `ADMIN` |

> El rol abre la **puerta**; la **pertenencia** (dueño) se valida siempre en el handler, no basta el rol.

---

# Extender un módulo existente (agregar un use case) + migración Java → .NET

Lo más común no es crear un módulo desde cero, sino **agregar un endpoint a un módulo que ya existe**
(p. ej. al migrar APIs de Java). Aquí solo escribes el **delta**: reutilizas entidad, DTO, controller y repo.

## Qué se agrega y qué NO se toca

| Agregas (delta) | No tocas |
|---|---|
| (si aplica) un método en el `I<Entidad>Repository` que ya existe + impl | la entidad |
| un Command/Query nuevo + handler en `UseCases/<Modulo>/` | DI / `InfrastructureServicesExtensions` |
| (si hay input con forma) un validador | el `UnitOfWork` (la propiedad del repo ya existe) |
| (si la salida cambia) un DTO de respuesta nuevo | los demás use cases del módulo |
| una **acción nueva** en el controller existente | la ruta base del controller |

> Si la entidad **no tenía repo propio** y tu nueva consulta lo necesita, ahí sí creas el repo
> (pasos 3b + 4 de esta guía) — esa es la única vez que el "delta" crece.

## Cómo saber a qué módulo pertenece

Se decide por la **entidad principal** del endpoint. La que **devuelves** manda: un endpoint que retorna
prendas filtradas → `UseCases/Prenda`, aunque la query cruce `categoria` o `marca`.

## Mapeo Java (Spring) → este proyecto

| Java (Spring) | Aquí (.NET) |
|---|---|
| método de `@RestController` | acción del `Controller` |
| `@Service` (la lógica) | el **Handler** del Command/Query |
| `@Repository` / `@Query` JPA / método derivado (`findByX`) | método en `I<Entidad>Repository` + impl EF |
| `@Valid` / Bean Validation | validador FluentValidation |
| `@PathVariable` | parámetro de ruta → `[JsonIgnore]` en el command |
| `@RequestParam` | `[FromQuery]` → propiedad del Query |
| `@RequestBody` | el Command (body) |
| `@PreAuthorize` / `hasRole` | `[Authorize(Roles = ...)]` |
| `ResponseEntity<>` / `HttpStatus` | `ApiResponse` + `this.ToActionResult` |
| `@Entity` (JPA) | ya existe en `Domain/Entities` |

## Ejemplo: nuevo endpoint con lógica de repositorio

Migrar `GET /api/prendas/buscar?categoriaId=&precioMax=` (en Java sería un `@GetMapping` + `@Service` +
un `@Query` en el `PrendaRepository`). El módulo es **Prenda** (es lo que devuelve).

**1) Método nuevo en el repo que ya existe** (la "lógica de repositorio"):

```csharp
// Domain/Ports/IRepositories/IPrendaRepository.cs   (AÑADIR una línea)
Task<(IEnumerable<Prenda> Items, int TotalCount)> Buscar(
    long? categoriaId, decimal? precioMax, int page, int pageSize);
```

```csharp
// Infrastructure/Adapters/Repositories/PrendaRepository.cs   (AÑADIR el método)
public async Task<(IEnumerable<Prenda> Items, int TotalCount)> Buscar(
    long? categoriaId, decimal? precioMax, int page, int pageSize)
{
    var q = _context.Set<Prenda>().AsNoTracking().Where(p => p.Activo);
    if (categoriaId is not null) q = q.Where(p => p.CategoriaId == categoriaId);
    if (precioMax  is not null) q = q.Where(p => p.Precio <= precioMax);

    var total = await q.CountAsync();
    var items = await q.OrderBy(p => p.Id)
        .Skip((Math.Max(page, 1) - 1) * pageSize).Take(pageSize).ToListAsync();
    return (items, total);
}
```

**2) Query + handler nuevos** (en el módulo Prenda; el `[FromQuery]` mapea a estas props):

```csharp
// UseCases/Prenda/Queries/BuscarPrendasQuery.cs
public class BuscarPrendasQuery : IRequest<ApiPaginationResponse<PrendaResponseDto>>
{
    public long? CategoriaId { get; set; }
    public decimal? PrecioMax { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class BuscarPrendasQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<BuscarPrendasQuery, ApiPaginationResponse<PrendaResponseDto>>
{
    public async Task<ApiPaginationResponse<PrendaResponseDto>> Handle(BuscarPrendasQuery request, CancellationToken ct)
    {
        var (items, total) = await _uow.Prendas.Buscar(
            request.CategoriaId, request.PrecioMax, request.Page, request.PageSize);

        // Sin resultados = 200 con data: [], nunca 404.
        return ApiPaginationResponse<PrendaResponseDto>.Ok(
            items.Select(p => new PrendaResponseDto { /* map */ }), total, request.Page, request.PageSize);
    }
}
```

**3) Validador** — solo si el filtro amerita rechazo (aquí los filtros son opcionales → normalmente **no** hace falta).

**4) Acción nueva en el controller que ya existe** (no se crea controller):

```csharp
// MixAndMatch.Api/Controllers/PrendasController.cs   (AÑADIR el método)
[HttpGet("buscar")]
[Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
public async Task<IActionResult> Buscar([FromQuery] BuscarPrendasQuery query) =>
    this.ToActionResult(await _mediator.Send(query));
```

Compila `Infrastructure` y `Application` (paso 8). Hecho: un endpoint nuevo sin tocar DI, UoW ni el resto del módulo.

## Plantilla de prompt para migrar un endpoint de Java

```text
Implementa este endpoint en el proyecto (viene de una API Java/Spring):

<pega aquí el método del @RestController + el @Service + la query del @Repository>

Reglas:
- Detecta a qué módulo de UseCases pertenece según la entidad que devuelve; NO crees un módulo
  nuevo si ya existe.
- Si la consulta no la cubre IGenericRepository, agrega un método a I<Entidad>Repository (+ impl EF).
  Si la entidad aún no tiene repo propio, créalo y exponlo en el UnitOfWork (no en DI).
- Crea el Command/Query + handler en ese módulo. ErrorType correcto (no todo 404); 201 al crear;
  lista vacía = 200 []; nada de GetAll()-en-memoria ni _mediator.Send dentro del handler.
- Si hay input con forma (body/filtros con reglas), agrega validador FluentValidation.
- Agrega la acción al controller existente con sus roles ([Authorize]) y this.ToActionResult.
- Respeta ESTANDAR_MODULOS.md y GUIA_NUEVO_MODULO.md.
- Antes de tocar nada, dame un plan corto (módulo destino, método de repo, validadores, cambios en
  el controller) y espera mi OK.
```

> Ese último punto (plan corto + esperar OK) es el mismo flujo con el que estandarizamos los módulos:
> evita que se invente algo que no encaje en el estándar.
