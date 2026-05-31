# Mix&Match — API REST de E-commerce de Ropa

API REST para una tienda de ropa en línea, desarrollada con **.NET 9** aplicando **Arquitectura Hexagonal** (Ports & Adapters) y el patrón **CQRS** mediante **MediatR**.

---

## Integrantes

| N° | Apellidos y Nombres |
|----|---------------------|
| 1 | Cama Quea, Luz Clarita |
| 2 | Estrada Perez, Alexander Willian |
| 3 | Guerra Pacheco, Geoge Miky |
| 4 | Huillca Pumachara, Yhon Italo |
| 5 | Quispe Quispe, Diego Zahid |
| 6 | Yucra Quispe, Angel Sebastian |

---

## Tecnologías

| Tecnología                         | Uso |
|------------------------------------|-----|
| .NET 9                             | Framework principal |
| ASP.NET Core Web API               | Capa de presentación HTTP |
| Entity Framework Core + PostgreSQL | Persistencia de datos |
| MediatR                            | CQRS — Commands y Queries |
| FluentValidation                   | Validación de requests |
| JWT + RSA                          | Autenticación y autorización |
| BCrypt.Net                         | Hash de contraseñas |
| SMTP Gmail                         | Envío de correos transaccionales |
| MercadoPago / Stripe / PayPal      | Pasarela de pagos (por definir) |
| Redis (Docker)                     | Caché distribuido en contenedor |
| Swashbuckle                        | Documentación Swagger |

---

## Arquitectura

El proyecto sigue la **Arquitectura Hexagonal (Ports & Adapters)**, organizada en 4 proyectos dentro de una sola solución:

```
MixAndMatch.sln
├── src/
│   ├── MixAndMatch.Domain/           ← Entidades, ValueObjects, Interfaces (Ports)
│   ├── MixAndMatch.Application/      ← Commands, Queries, Handlers, DTOs
│   ├── MixAndMatch.Infrastructure/   ← Repositorios EF Core, Servicios externos (Adapters)
│   └── MixAndMatch.Api/              ← Controllers, Program.cs, configuración
└── tests/
```

**Regla de dependencias:**
```
Api → Infrastructure → Application → Domain
```

Domain no conoce ninguna capa externa. Infrastructure implementa los contratos que Domain define.

---

## Módulos

| Módulo | Descripción |
|--------|-------------|
| **Identity** | Registro, login con JWT, autenticación con Google, recuperación de contraseña |
| **Catalog** | Prendas, categorías, marcas, proveedores, tallas, imágenes |
| **Cart** | Carrito de compras por usuario |
| **Orders** | Ciclo de vida de ventas y pedidos |
| **Payment** | Métodos de pago y procesamiento (MercadoPago / Stripe / PayPal) |
| **Shipping** | Envíos, tracking y datos de destinatario |
| **Discounts** | Descuentos por prenda, categoría y códigos promocionales |
| **Reviews** | Resenias y calificaciones de prendas con moderacion |
| **Notifications** | Envío de emails transaccionales (bienvenida, confirmación, tracking, reset) |

---

## Requisitos previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) y Docker Compose
- PostgreSQL no hace falta instalarlo localmente si usas `docker compose`

---

## Configuración

1. Clonar el repositorio:

```bash
git clone <url-del-repo>
cd MixAndMatch
```

2. Levantar la base de datos y Redis con Docker Compose:

```bash
docker compose up -d
```

3. Configurar `appsettings.Development.json` en `MixAndMatch.Api/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5435;Database=mixandmatch;Username=postgres;Password=postgres"
  },
  "AllowedHosts": "*"
}
```

4. Aplicar migraciones:

```bash
dotnet ef database update --project MixAndMatch.Infrastructure --startup-project MixAndMatch.Api
```

5. Ejecutar:

```bash
dotnet run --project MixAndMatch.Api
```

6. Acceder a Swagger:

```
http://localhost:5221
```

### Docker Compose

El archivo `docker-compose.yml` levanta estos servicios:

- PostgreSQL en `localhost:5435`
- Redis en `localhost:6380`

El script de inicialización de PostgreSQL vive en `docker/postgres/init.sql` y se monta automáticamente en el contenedor.

Para revisar el estado de los contenedores:

```bash
docker compose ps
```

Para ver logs:

```bash
docker compose logs -f
```

---

## Estructura de carpetas por capa

### Domain
```
Entities/        ← todas las entidades del sistema
ValueObjects/    ← objetos de valor con igualdad estructural
Ports/
    Repositories/    ← interfaces de repositorios (ports de persistencia)
    Services/        ← interfaces de servicios externos (ports: email, pago, storage)
Common/          ← Entity, AggregateRoot, ValueObject, IRepository, PageResult
```

### Application
```
UsesCases/
    Commands/        ← un directorio por comando (Command + Handler)
    Queries/         ← un directorio por query (Query + Handler)
Common/          ← IUnitOfWork, PageResponseDto, Pipeline Behaviors
```

### Infrastructure
```
Adapters/
  Repositories/  ← implementaciones EF Core de los repositorios
  Services/      ← implementaciones de servicios externos (SMTP Gmail, pasarela de pagos, BCrypt...)
Configurations/ Registro de servicios
Middleware/    ← manejo global de errores, rate limiting, headers de seguridad
Common/
  Caching/       ← servicio de Redis
```

### Api
```
Controllers/     ← todos los controllers de todos los módulos
```
