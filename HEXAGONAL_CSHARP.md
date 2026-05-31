# Mix&Match — Arquitectura Hexagonal en C# / .NET 8

---

## Tabla de Contenidos

1. [Principios de la Arquitectura Hexagonal](#1-principios-de-la-arquitectura-hexagonal)
2. [Proyectos de la Solución](#2-proyectos-de-la-solución)
3. [Estructura de Carpetas por Capa](#3-estructura-de-carpetas-por-capa)
4. [Módulos — Archivos por Módulo](#4-módulos--archivos-por-módulo)
   - [4.1 Identity](#41-identity--autenticación--usuarios)
   - [4.2 Catalog](#42-catalog--catálogo-de-prendas)
   - [4.3 Cart](#43-cart--carrito-de-compras)
   - [4.4 Orders](#44-orders--ventas--pedidos)
   - [4.5 Payment](#45-payment--pagos)
   - [4.6 Shipping](#46-shipping--envíos)
   - [4.7 Discounts](#47-discounts--descuentos)
   - [4.8 Reviews](#48-reviews--reseñas)
   - [4.9 Notifications](#49-notifications--notificaciones)
5. [Capa API — Ensamblado Final](#5-capa-api--ensamblado-final)
6. [Dependencias NuGet](#6-dependencias-nuget)
7. [Correspondencia Java → C#](#7-correspondencia-java--c)

---

## 1. Principios de la Arquitectura Hexagonal

```
┌──────────────────────────────────────────────────┐
│             INFRASTRUCTURE (Adapters)            │
│   Controllers · Repositories EF Core · Gateways │
│  ┌────────────────────────────────────────────┐  │
│  │          APPLICATION (Use Cases)           │  │
│  │        Commands · Queries · DTOs           │  │
│  │  ┌──────────────────────────────────────┐  │  │
│  │  │           DOMAIN (Core)              │  │  │
│  │  │  Entities · ValueObjects             │  │  │
│  │  │  Ports/Repositories (interfaces)     │  │  │
│  │  │  Ports/Services (interfaces)         │  │  │
│  │  └──────────────────────────────────────┘  │  │
│  └────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘
```

| Capa | Responsabilidad |
|------|----------------|
| **Domain** | Entidades, Value Objects, interfaces de Repositories (Ports) y Services externos (Ports) |
| **Application** | Commands, Queries, Handlers (MediatR), DTOs |
| **Infrastructure** | Adapters: Repositories EF Core, Services externos (SMTP, JWT, pagos...), Middleware |
| **Api** | Host ASP.NET Core — ensambla todo, registra DI, configura middleware |

**Regla de dependencias — las flechas siempre apuntan hacia adentro:**
```
Api  →  Infrastructure  →  Application  →  Domain
```

---

## 2. Proyectos de la Solución

```
MixAndMatch.sln
│
├── src/
│   ├── MixAndMatch.Domain/           ← classlib
│   ├── MixAndMatch.Application/      ← classlib
│   ├── MixAndMatch.Infrastructure/   ← classlib
│   └── MixAndMatch.Api/              ← webapi (único ejecutable)
│
└── tests/
    ├── MixAndMatch.Domain.UnitTests/
    └── MixAndMatch.Application.UnitTests/
```

**Comandos para crear la solución:**

```bash
dotnet new sln -n MixAndMatch
dotnet new classlib -n MixAndMatch.Domain         -o src/MixAndMatch.Domain
dotnet new classlib -n MixAndMatch.Application    -o src/MixAndMatch.Application
dotnet new classlib -n MixAndMatch.Infrastructure -o src/MixAndMatch.Infrastructure
dotnet new webapi   -n MixAndMatch.Api            -o src/MixAndMatch.Api

dotnet sln add src/MixAndMatch.Domain
dotnet sln add src/MixAndMatch.Application
dotnet sln add src/MixAndMatch.Infrastructure
dotnet sln add src/MixAndMatch.Api

dotnet add src/MixAndMatch.Application    reference src/MixAndMatch.Domain
dotnet add src/MixAndMatch.Infrastructure reference src/MixAndMatch.Application
dotnet add src/MixAndMatch.Infrastructure reference src/MixAndMatch.Domain
dotnet add src/MixAndMatch.Api            reference src/MixAndMatch.Infrastructure
dotnet add src/MixAndMatch.Api            reference src/MixAndMatch.Application
```

---

## 3. Estructura de Carpetas por Capa

### MixAndMatch.Domain

```
MixAndMatch.Domain/
│
├── Entities/                    ← entidades + value objects + clases base
│   ├── Entity.cs                ← Id (Guid), igualdad por Id
│   ├── AggregateRoot.cs         ← extiende Entity, emite Domain Events
│   ├── ValueObject.cs           ← igualdad estructural por valor
│   └── PageResult.cs            ← TotalCount, Items, Page, PageSize
└── Ports/
    ├── Repositories/            ← interfaces de repositorios (ports de persistencia)
    │   ├── IRepository.cs       ← FindById, Save, Delete genérico
    │   └── IUnitOfWork.cs       ← CommitAsync
    └── Services/                ← interfaces de servicios externos (ports: email, pago, storage)
```

### MixAndMatch.Application

```
MixAndMatch.Application/
│
├── UseCases/
│   └── [Entidad]/               ← un directorio por entidad
│       ├── Commands/
│       │   └── CreateXxxCommand.cs   ← record Command + class Handler en un solo archivo
│       └── Queries/
│           └── GetXxxQuery.cs        ← record Query + class Handler en un solo archivo
└── DTOs/                        ← todos los DTOs organizados por módulo
```

### MixAndMatch.Infrastructure

```
MixAndMatch.Infrastructure/
│
├── Adapters/
│   ├── Repositories/            ← implementaciones EF Core + AppDbContext + EF Configurations + Migrations
│   └── Services/                ← implementaciones de servicios externos (SMTP, pagos, BCrypt...)
├── Configurations/              ← registro de servicios (DI extensions)
└── Middleware/
    ├── GlobalExceptionMiddleware.cs
    ├── RateLimitMiddleware.cs
    └── SecurityHeadersMiddleware.cs
```

### MixAndMatch.Api

```
MixAndMatch.Api/
│
├── Controllers/                 ← todos los Controllers de todos los módulos
├── Program.cs                   ← registra todo: MediatR, EF, JWT, Swagger...
├── appsettings.json
└── appsettings.Development.json
```

---

## 4. Módulos — Archivos por Módulo

> 22 tablas distribuidas en 8 módulos + 1 módulo sin tabla (Notifications).
> Los `*Configuration.cs` son Fluent API de EF Core y viven junto a su repositorio.

---

### 4.1 Identity — Autenticación & Usuarios

**Tablas:** `usuarios` (1 tabla)

**Domain**
```
Entities/
  Usuario.cs               ← id, nombre_usuario, email, contrasenia, rol, activo (AggregateRoot)
  Email.cs                 ← VO: string validado con formato email
  PasswordHash.cs          ← VO: string no vacío, formato hash bcrypt
  Rol.cs                   ← enum: Admin | Cliente

Ports/Repositories/
  IUsuarioRepository.cs    ← FindByEmail, FindByNombreUsuario, FindById, Save, Delete

Ports/Services/
  IForgotCodeStore.cs      ← GuardarToken, ValidarToken, Invalidar
  IJwtService.cs           ← GenerarToken, ValidarToken
  IPasswordHasher.cs       ← HashPassword, VerifyPassword
```

**Application**
```
UseCases/
  Usuario/
    Commands/
      RegisterUsuarioCommand.cs      ← Command + Handler (crea usuario + publica SendWelcomeEmailCommand)
      UpdateUsuarioCommand.cs        ← Command + Handler
      DeleteUsuarioCommand.cs        ← Command + Handler
      ForgotPasswordCommand.cs       ← Command + Handler (guarda token + publica SendResetPasswordEmailCommand)
      ResetPasswordCommand.cs        ← Command + Handler
    Queries/
      LoginUsuarioQuery.cs           ← Query + Handler (valida credenciales + genera JWT)
      GetUsuarioByIdQuery.cs         ← Query + Handler
      ListUsuariosQuery.cs           ← Query + Handler (solo Admin)

DTOs/
  UsuarioDto.cs
  CreateUsuarioRequest.cs
  UpdateUsuarioRequest.cs
  LoginRequest.cs
  LoginResponse.cs              ← { token, expira, usuario }
```

**Infrastructure**
```
Adapters/Repositories/
  UsuarioRepository.cs          ← implementa IUsuarioRepository (EF Core)
  UsuarioConfiguration.cs       ← Fluent API: tabla usuarios, índices email+nombre_usuario

Adapters/Services/
  JwtService.cs                 ← implementa IJwtService
  GoogleOAuthService.cs         ← valida token Google OAuth
  ForgotCodeStore.cs            ← implementa IForgotCodeStore (Redis)
  BcryptPasswordHasher.cs       ← implementa IPasswordHasher
```

**Api/Controllers/**
```
AuthController.cs          ← POST /api/auth/login, /register, /google, /forgot, /reset
UsuarioController.cs       ← GET /api/usuarios, GET /{id}, PUT /{id}, DELETE /{id}
```

---

### 4.2 Catalog — Catálogo de Prendas

**Tablas:** `prenda`, `prenda_talla`, `prenda_imagen`, `categoria`, `genero`, `marca`, `proveedor`, `talla` (8 tablas)

**Domain**
```
Entities/
  Prenda.cs                ← id, nombre, descripcion, marcaId, categoriaId, proveedorId,
                              generoId, precio decimal, activo (AggregateRoot)
  PrendaTalla.cs           ← id, prendaId, tallaId, stock int  (índice único: prendaId+tallaId)
  PrendaImagen.cs          ← id, prendaId, tipo, url, orden
  Categoria.cs             ← id, nom_categoria
  Genero.cs                ← id, nom_genero
  Marca.cs                 ← id, nom_marca
  Proveedor.cs             ← id, nom_proveedor
  Talla.cs                 ← id, nom_talla  (XS·S·M·L·XL·XXL)
  TipoImagen.cs            ← enum: principal | hover | galeria | video

Ports/Repositories/
  IPrendaRepository.cs          ← FindById, Search (paginado + filtros), Save, Delete
  IPrendaTallaRepository.cs     ← FindByPrendaId, FindByPrendaYTalla, Save, UpdateStock
  IPrendaImagenRepository.cs    ← FindByPrendaId, Save, Delete
  ICategoriaRepository.cs
  IGeneroRepository.cs
  IMarcaRepository.cs
  IProveedorRepository.cs
  ITallaRepository.cs

Ports/Services/
  IArchivoStorage.cs       ← SubirArchivo, EliminarArchivo, ObtenerUrl
```

**Application**
```
UseCases/
  Prenda/
    Commands/
      CreatePrendaCommand.cs             ← Command + Handler
      UpdatePrendaCommand.cs             ← Command + Handler
      DeletePrendaCommand.cs             ← Command + Handler
      AddPrendaTallaCommand.cs           ← Command + Handler
      UpdateStockPrendaTallaCommand.cs   ← Command + Handler (llamado al confirmar/cancelar venta)
      UploadPrendaImagenCommand.cs       ← Command + Handler
      DeletePrendaImagenCommand.cs       ← Command + Handler
    Queries/
      GetPrendaByIdQuery.cs              ← Query + Handler
      SearchPrendasQuery.cs              ← Query + Handler (filtros: categoriaId, generoId, marcaId, tallaId, precioMin/Max)
      GetPrendaTallasByPrendaQuery.cs    ← Query + Handler
  Categoria/
    Commands/
      CreateCategoriaCommand.cs          ← Command + Handler
    Queries/
      ListCategoriasQuery.cs             ← Query + Handler
  Genero/
    Commands/
      CreateGeneroCommand.cs             ← Command + Handler
    Queries/
      ListGenerosQuery.cs                ← Query + Handler
  Marca/
    Commands/
      CreateMarcaCommand.cs              ← Command + Handler
    Queries/
      ListMarcasQuery.cs                 ← Query + Handler
  Proveedor/
    Commands/
      CreateProveedorCommand.cs          ← Command + Handler
    Queries/
      ListProveedoresQuery.cs            ← Query + Handler
  Talla/
    Commands/
      CreateTallaCommand.cs              ← Command + Handler
    Queries/
      ListTallasQuery.cs                 ← Query + Handler

DTOs/
  PrendaDto.cs
  PrendaRequestDto.cs
  PrendaTallaDto.cs
  PrendaImagenDto.cs
  FiltroPrendaDto.cs
  CategoriaDto.cs
  GeneroDto.cs
  MarcaDto.cs
  ProveedorDto.cs
  TallaDto.cs
```

**Infrastructure**
```
Adapters/Repositories/
  PrendaRepository.cs
  PrendaTallaRepository.cs
  PrendaImagenRepository.cs
  CategoriaRepository.cs
  GeneroRepository.cs
  MarcaRepository.cs
  ProveedorRepository.cs
  TallaRepository.cs
  PrendaConfiguration.cs          ← Fluent API: FK marca/categoria/proveedor/genero
  PrendaTallaConfiguration.cs     ← Fluent API: índice único (prenda_id, talla_id)
  PrendaImagenConfiguration.cs    ← Fluent API: enum tipo
  CategoriaConfiguration.cs
  GeneroConfiguration.cs
  MarcaConfiguration.cs
  TallaConfiguration.cs

Adapters/Services/
  LocalArchivoStorage.cs          ← implementa IArchivoStorage
```

**Api/Controllers/**
```
PrendaController.cs
PrendaTallaController.cs
PrendaImagenController.cs
CategoriaController.cs
GeneroController.cs
MarcaController.cs
ProveedorController.cs
TallaController.cs
ArchivoController.cs              ← upload/delete archivos físicos
```

---

### 4.3 Cart — Carrito de Compras

**Tablas:** `carrito`, `carrito_item` (2 tablas)

**Domain**
```
Entities/
  Carrito.cs               ← id, usuarioId, fechaCreacion, estado, updatedAt (AggregateRoot)
  CarritoItem.cs           ← id, carritoId, prendaTallaId, precioUnitario, cantidad int
                              (índice único: carritoId + prendaTallaId)
  EstadoCarrito.cs         ← enum: ACTIVO | CANCELADO

Ports/Repositories/
  ICarritoRepository.cs     ← FindActivoByUsuarioId, Save, UpdateEstado
  ICarritoItemRepository.cs ← FindByCarritoId, FindByCarritoYPrendaTalla, Save, Delete
```

**Application**
```
UseCases/
  Carrito/
    Commands/
      AddItemToCarritoCommand.cs       ← Command + Handler
      UpdateItemCantidadCommand.cs     ← Command + Handler
      RemoveItemFromCarritoCommand.cs  ← Command + Handler
      ClearCarritoCommand.cs           ← Command + Handler (estado → CANCELADO al crear venta)
    Queries/
      GetCarritoActivoByUsuarioQuery.cs ← Query + Handler

DTOs/
  CarritoDto.cs
  CarritoItemDto.cs
  AddCarritoItemRequest.cs
  UpdateCantidadRequest.cs
```

**Infrastructure**
```
Adapters/Repositories/
  CarritoRepository.cs
  CarritoItemRepository.cs
  CarritoConfiguration.cs         ← Fluent API: FK usuarioId
  CarritoItemConfiguration.cs     ← Fluent API: índice único (carrito_id, prenda_talla_id)
```

**Api/Controllers/**
```
CarritoController.cs
CarritoItemController.cs
```

---

### 4.4 Orders — Ventas & Pedidos

**Tablas:** `ventas`, `ventas_detalle` (2 tablas)

**Domain**
```
Entities/
  Venta.cs                 ← id, usuarioId, fechaCreacion, estado, updatedAt (AggregateRoot)
  VentaDetalle.cs          ← id, ventaId, prendaTallaId, cantidad, precioUnitario decimal
                              (índice único: ventaId + prendaTallaId)
  EstadoVenta.cs           ← enum: PENDIENTE | PROCESANDO | ENVIADO | ENTREGADO | CANCELADO

Ports/Repositories/
  IVentaRepository.cs       ← FindById, FindByUsuarioId, FindAll (paginado), Save, UpdateEstado
  IVentaDetalleRepository.cs ← FindByVentaId, Save
```

**Application**
```
UseCases/
  Venta/
    Commands/
      CreateVentaCommand.cs        ← Command + Handler (carritoId + metodoPagoId + datosEnvioId,
                                      crea venta + detalles + publica UpdateStockPrendaTallaCommand
                                      + ClearCarritoCommand + SendConfirmacionVentaEmailCommand)
      UpdateEstadoVentaCommand.cs  ← Command + Handler
      CancelVentaCommand.cs        ← Command + Handler (revierte stock)
    Queries/
      GetVentaByIdQuery.cs         ← Query + Handler
      ListVentasByUsuarioQuery.cs  ← Query + Handler
      ListAllVentasQuery.cs        ← Query + Handler (solo Admin, paginado)

DTOs/
  VentaDto.cs
  VentaRequestDto.cs
  VentaDetalleDto.cs
```

**Infrastructure**
```
Adapters/Repositories/
  VentaRepository.cs
  VentaDetalleRepository.cs
  VentaConfiguration.cs           ← Fluent API: FK usuarioId
  VentaDetalleConfiguration.cs    ← Fluent API: índice único (venta_id, prenda_talla_id)
```

**Api/Controllers/**
```
VentaController.cs
VentaDetalleController.cs
```

---

### 4.5 Payment — Pagos

**Tablas:** `pago`, `metodo_pago` (2 tablas)

**Domain**
```
Entities/
  Pago.cs                  ← id, ventaId, metodoId, monto decimal, estado, fechaCreacion (AggregateRoot)
                              (N:1 con ventas → permite reintentos tras RECHAZADO)
  MetodoPago.cs            ← id, tipoPago  (catálogo: TARJETA_CREDITO | TARJETA_DEBITO | BILLETERA)
  EstadoPago.cs            ← enum: PENDIENTE | PAGADO | RECHAZADO | REEMBOLSADO
  TipoPago.cs              ← enum: TARJETA_CREDITO | TARJETA_DEBITO | BILLETERA

Ports/Repositories/
  IPagoRepository.cs       ← FindById, FindByVentaId, FindUltimoPagado, Save
  IMetodoPagoRepository.cs ← FindAll, FindById, Save, Delete

Ports/Services/
  IPaymentGateway.cs       ← ProcesarPago, ReembolsarPago
```

**Application**
```
UseCases/
  Pago/
    Commands/
      ProcessPagoCommand.cs        ← Command + Handler (ventaId + metodoId + monto → llama IPaymentGateway)
      RefundPagoCommand.cs         ← Command + Handler
    Queries/
      GetPagosByVentaQuery.cs      ← Query + Handler (historial con reintentos)
  MetodoPago/
    Commands/
      CreateMetodoPagoCommand.cs   ← Command + Handler
      DeleteMetodoPagoCommand.cs   ← Command + Handler
    Queries/
      ListMetodosPagoQuery.cs      ← Query + Handler

DTOs/
  PagoDto.cs
  PagoRequestDto.cs
  MetodoPagoDto.cs
```

**Infrastructure**
```
Adapters/Repositories/
  PagoRepository.cs
  MetodoPagoRepository.cs
  PagoConfiguration.cs            ← Fluent API: FK ventaId, metodoId
  MetodoPagoConfiguration.cs      ← Fluent API: unique tipo_pago

Adapters/Services/
  StripePaymentGateway.cs         ← implementa IPaymentGateway
```

**Api/Controllers/**
```
PagoController.cs
MetodoPagoController.cs
```

---

### 4.6 Shipping — Envíos

**Tablas:** `envio`, `datos_envio` (2 tablas)

**Domain**
```
Entities/
  Envio.cs                 ← id, ventaId, datosEnvioId, costoEnvio decimal, fechaEnvio,
                              fechaEntrega, estado, metodoEnvio, trackingNumber (AggregateRoot)
  DatosEnvio.cs            ← id, usuarioId, nombres, apellidos, dni, departamento,
                              provincia, distrito, calle, detalle, telefono, email, esPrincipal
  EstadoEnvio.cs           ← enum: PENDIENTE | EN_CAMINO | ENTREGADO | DEVUELTO
  MetodoEnvio.cs           ← enum: ESTANDAR | EXPRESS | RECOJO_TIENDA

Ports/Repositories/
  IEnvioRepository.cs      ← FindById, FindByVentaId, Save, UpdateEstado
  IDatosEnvioRepository.cs ← FindById, FindByUsuarioId, FindPrincipalByUsuarioId, Save, Delete

Ports/Services/
  IShippingProvider.cs     ← ObtenerTracking, CalcularCosto, CalcularFechaEstimada
```

**Application**
```
UseCases/
  Envio/
    Commands/
      CreateEnvioCommand.cs              ← Command + Handler (ventaId + datosEnvioId + metodoEnvio)
      UpdateEstadoEnvioCommand.cs        ← Command + Handler (publica SendTrackingEmailCommand)
    Queries/
      GetEnvioByVentaQuery.cs            ← Query + Handler
      TrackEnvioQuery.cs                 ← Query + Handler
  DatosEnvio/
    Commands/
      CreateDatosEnvioCommand.cs         ← Command + Handler
      UpdateDatosEnvioCommand.cs         ← Command + Handler
      DeleteDatosEnvioCommand.cs         ← Command + Handler
      SetDatosEnvioPrincipalCommand.cs   ← Command + Handler
    Queries/
      ListDatosEnvioByUsuarioQuery.cs    ← Query + Handler

DTOs/
  EnvioDto.cs
  EnvioRequestDto.cs
  DatosEnvioDto.cs
  CreateDatosEnvioRequest.cs
```

**Infrastructure**
```
Adapters/Repositories/
  EnvioRepository.cs
  DatosEnvioRepository.cs
  EnvioConfiguration.cs           ← Fluent API: FK ventaId, datosEnvioId
  DatosEnvioConfiguration.cs      ← Fluent API: FK usuarioId, índice parcial esPrincipal

Adapters/Services/
  MockShippingProvider.cs         ← implementa IShippingProvider
```

**Api/Controllers/**
```
EnvioController.cs
DatosEnvioController.cs
```

---

### 4.7 Discounts — Descuentos

**Tablas:** `descuento_prenda`, `descuento_categoria`, `descuento_codigo`, `descuento_usuario` (4 tablas)

**Domain**
```
Entities/
  DescuentoPrenda.cs       ← id, prendaId, porcentaje decimal, fechaInicio, fechaFin, activo
  DescuentoCategoria.cs    ← id, categoriaId, porcentaje decimal, fechaInicio, fechaFin, activo
  DescuentoCodigo.cs       ← id, codigo, descripcion, porcentaje decimal, fechaInicio, fechaFin,
                              usoMaximo int, activo  (sin campo "usado" — se calcula desde descuento_usuario)
  DescuentoUsuario.cs      ← id, descuentoCodigoId, usuarioId, fechaUso
                              (índice único: descuentoCodigoId + usuarioId)
  CodigoPromo.cs           ← VO: string validado (max 50 chars, sin espacios)

Ports/Repositories/
  IDescuentoPrendaRepository.cs    ← FindByPrendaId, FindActivos, Save, Delete
  IDescuentoCategoriaRepository.cs ← FindByCategoriaId, FindActivos, Save, Delete
  IDescuentoCodigoRepository.cs    ← FindByCodigo, FindActivos, Save, CountUsos
  IDescuentoUsuarioRepository.cs   ← FindByCodigoYUsuario, CountByCodigoId, Save
```

**Application**
```
UseCases/
  DescuentoPrenda/
    Commands/
      CreateDescuentoPrendaCommand.cs          ← Command + Handler
    Queries/
      GetDescuentosByPrendaQuery.cs            ← Query + Handler
      GetPrendasConDescuentoQuery.cs           ← Query + Handler
  DescuentoCategoria/
    Commands/
      CreateDescuentoCategoriaCommand.cs       ← Command + Handler
    Queries/
      GetDescuentosByCategoriaQuery.cs         ← Query + Handler
  DescuentoCodigo/
    Commands/
      CreateDescuentoCodigoCommand.cs          ← Command + Handler
      AplicarCodigoDescuentoCommand.cs         ← Command + Handler (valida límite + registra uso)
      DesactivarDescuentosExpiradosCommand.cs  ← Command + Handler (ejecutado por IHostedService)
    Queries/
      ValidarCodigoDescuentoQuery.cs           ← Query + Handler (activo + no expirado + usos < máximo)

DTOs/
  DescuentoPrendaDto.cs
  DescuentoCategoriaDto.cs
  DescuentoCodigoDto.cs
  AplicarDescuentoRequest.cs
  ValidarCodigoResponse.cs
  PrendaConDescuentoDto.cs
```

**Infrastructure**
```
Adapters/Repositories/
  DescuentoPrendaRepository.cs
  DescuentoCategoriaRepository.cs
  DescuentoCodigoRepository.cs
  DescuentoUsuarioRepository.cs
  DescuentoPrendaConfiguration.cs     ← Fluent API: FK prendaId
  DescuentoCategoriaConfiguration.cs  ← Fluent API: FK categoriaId
  DescuentoCodigoConfiguration.cs     ← Fluent API: unique codigo
  DescuentoUsuarioConfiguration.cs    ← Fluent API: índice único (descuento_codigo_id, usuario_id)

Adapters/Services/
  DescuentoExpirationJob.cs           ← IHostedService → publica DesactivarDescuentosExpiradosCommand
```

**Api/Controllers/**
```
DescuentoPrendaController.cs
DescuentoCategoriaController.cs
DescuentoCodigoController.cs
DescuentoUsuarioController.cs
```

---

### 4.8 Reviews — Reseñas

**Tablas:** `resenia` (1 tabla)

**Domain**
```
Entities/
  Resenia.cs               ← id, prendaId, usuarioId, calificacion, comentario, estado, moderadoPorId,
                              moderadoEn, motivoRechazo, createdAt, updatedAt (AggregateRoot)
                              (indice unico: prendaId + usuarioId — un usuario, una resenia por prenda)
  Calificacion.cs          ← VO: int validado 1–5

Enums/
  EstadoResenia.cs         ← PENDIENTE, APROBADA, RECHAZADA

Ports/
  IReseniaRepository.cs    ← GetPaginatedByPrendaId, GetByUsuarioId, GetByPrendaAndUsuario,
                              GetPromedioByPrendaId, Add, Update, Delete

DTOs/Resenias/
  ReseniaResponseDto.cs
  ReseniaResumenDto.cs     ← promedio + total resenias por prenda
```

**Application**
```
UseCases/
  Resenias/
    Commands/
      CreateReseniaCommand.cs      ← Command + Handler
      UpdateReseniaCommand.cs      ← Command + Handler
      DeleteReseniaCommand.cs      ← Command + Handler
      UpdateEstadoReseniaCommand.cs← Command + Handler
    Queries/
      GetReseniasByPrendaQuery.cs  ← Query + Handler (paginado + promedio)
      GetReseniasByUsuarioQuery.cs ← Query + Handler
      GetReseniaByIdQuery.cs       ← Query + Handler
```

**Infrastructure**
```
Adapters/Repositories/
  ReseniaRepository.cs

Configurations/
  ReseniaConfiguration.cs          ← Fluent API: estado como string, indice unico (prenda_id, usuario_id)
```

**Api/Controllers/**
```
ReseniasController.cs
```

---

### 4.9 Notifications — Notificaciones

**Tablas:** ninguna (módulo transversal, sin persistencia propia)

> Solo define el contrato (port) y su implementación (adapter).
> Los Handlers de otros módulos publican el Command de email vía MediatR.

**Domain**
```
Ports/Services/
  IEmailSender.cs          ← Send(to, subject, body)   ← PORT
```

**Application**
```
UseCases/
  Notification/
    Commands/
      SendWelcomeEmailCommand.cs           ← Command + Handler ({ email, nombre })
      SendResetPasswordEmailCommand.cs     ← Command + Handler ({ email, token })
      SendConfirmacionVentaEmailCommand.cs ← Command + Handler ({ email, ventaId, detalle })
      SendTrackingEmailCommand.cs          ← Command + Handler ({ email, trackingNumber, estado })
```

**Infrastructure**
```
Adapters/Services/
  SmtpEmailSender.cs       ← implementa IEmailSender (SMTP Gmail)   ← ADAPTER
```

**Api/Controllers/**
```
MailController.cs          ← POST /api/mail/send  (solo Admin, envío manual de prueba)
```

---

## 5. Capa API — Ensamblado Final

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();        // MediatR + FluentValidation
builder.Services.AddInfrastructure(builder.Configuration);  // EF Core + JWT + Redis + ...
builder.Services.AddControllers();
builder.Services.AddSwaggerSetup();

var app = builder.Build();

app.UseGlobalExceptionMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

```csharp
// Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
    services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    return services;
}
```

```csharp
// Infrastructure/Configurations/InfrastructureServices.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(config.GetConnectionString("Default")));

    services.AddStackExchangeRedisCache(opt =>
        opt.Configuration = config.GetConnectionString("Redis"));

    services.AddScoped<IUsuarioRepository, UsuarioRepository>();
    services.AddScoped<IPrendaRepository, PrendaRepository>();
    // ... resto de repositorios
    services.AddScoped<IUnitOfWork, AppUnitOfWork>();

    services.AddScoped<IEmailSender, SmtpEmailSender>();
    services.AddScoped<IPaymentGateway, StripePaymentGateway>();
    services.AddScoped<IArchivoStorage, LocalArchivoStorage>();
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

    services.AddHostedService<DescuentoExpirationJob>();
    return services;
}
```

---

## 6. Dependencias NuGet

| Función | Proyecto | Paquete |
|---------|----------|---------|
| Mediator (Commands/Queries) | Application + Api | `MediatR` |
| Validación | Application | `FluentValidation` + `FluentValidation.DependencyInjectionExtensions` |
| ORM | Infrastructure | `Microsoft.EntityFrameworkCore` |
| PostgreSQL | Infrastructure | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| JWT | Infrastructure + Api | `Microsoft.AspNetCore.Authentication.JwtBearer` |
| Redis (Docker) | Infrastructure | `StackExchange.Redis` |
| Email SMTP Gmail | Infrastructure | `System.Net.Mail` (built-in .NET) |
| Hash contraseñas | Infrastructure | `BCrypt.Net-Next` |
| Pasarela de pagos (por definir) | Infrastructure | MercadoPago / Stripe.net / PayPalCheckoutSdk |
| Mapeo DTOs | Application | `Mapster` |
| Documentación | Api | `Swashbuckle.AspNetCore` |
| Tests | Tests | `xUnit` + `Moq` + `Testcontainers` |

---

## 7. Correspondencia Java → C#

| Concepto Java | Equivalente C# / .NET |
|--------------|----------------------|
| `@RestController` + `@RequestMapping` | `[ApiController]` + `[Route("api/[controller]")]` |
| `@Service` (Interface + Impl) | `IRequestHandler<TCommand, TResult>` (MediatR Handler) |
| `CrudRepository<T, ID>` + `@Query` | `DbSet<T>` + LINQ / `.FromSqlRaw()` |
| `@Entity` + `@Table` | Fluent API en `OnModelCreating` |
| `@Scheduled(fixedRate)` | `IHostedService` |
| `@Component` seeder | `IHostedService` en startup |
| Lombok `@Data`, `@Builder` | C# `record` / `init` properties |
| `ResponseEntity<T>` | `IActionResult` / `Results<T>` |
| `Optional<T>` | `T?` (nullable reference types) |
| Spring Security `SecurityConfig` | `AddAuthentication()` + `AddAuthorization()` |
| `@PreAuthorize("hasRole('ADMIN')")` | `[Authorize(Roles = "Admin")]` |
| `@EnableCaching` + `@Cacheable` | `IDistributedCache` con prefijos de clave |
| JWT filter chain | `AddJwtBearer()` middleware |
| Google OAuth | `AddGoogle()` en `AddAuthentication()` |
| `ApplicationEventPublisher` | `IPublisher` de MediatR (`.Publish(domainEvent)`) |
| `@EventListener` | `INotificationHandler<TEvent>` de MediatR |