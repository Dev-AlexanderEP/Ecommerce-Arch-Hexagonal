# Módulo Prendas

Gestiona el catálogo de prendas y su inventario por talla. Ambos controllers (`PrendasController` y `PrendaTallasController`) forman un único módulo según la arquitectura hexagonal del proyecto. Todos los endpoints devuelven el formato descrito en [api-responses.md](./api-responses.md).

**Base URLs:**
- Prendas: `/api/prendas`
- Inventario: `/api/prendatallas`

> **Nota de migración (frontend Java → .NET):** Las rutas del backend anterior (Spring) no coinciden con las de esta API. Ver la [tabla de mapeo](#mapeo-de-rutas-java--net) al final de este documento.

---

## Endpoints — Prendas

| Método | Ruta | Autorización | Descripción |
|--------|------|-------------|-------------|
| GET | `/api/prendas` | Autenticado | Lista paginada de todas las prendas. |
| GET | `/api/prendas/{id}` | Autenticado | Prenda por ID. |
| POST | `/api/prendas` | ADMIN | Crear nueva prenda. |
| PUT | `/api/prendas/{id}` | ADMIN | Actualizar prenda. |
| DELETE | `/api/prendas/{id}` | ADMIN | Eliminar prenda. |
| GET | `/api/prendas/tallas-por-genero/{genero}` | Autenticado | Tallas disponibles para un género. |
| GET | `/api/prendas/marcas-por-genero/{genero}` | Autenticado | Marcas disponibles para un género. |
| GET | `/api/prendas/categorias-por-genero/{genero}` | Autenticado | Categorías disponibles para un género. |
| GET | `/api/prendas/precios-por-genero/{genero}` | Autenticado | Estadísticas de precios para un género. |
| GET | `/api/prendas/descuentos-por-genero/{genero}` | Autenticado | Descuentos disponibles para un género. |
| GET | `/api/prendas/con-descuentos-aleatorio/{genero}` | Autenticado | Prendas con descuento aleatorio por género. |
| GET | `/api/prendas/tallas-por-categoria/{categoria}` | Autenticado | Tallas disponibles para una categoría. |
| GET | `/api/prendas/marcas-por-categoria/{categoria}` | Autenticado | Marcas disponibles para una categoría. |
| GET | `/api/prendas/precios-por-categoria/{categoria}` | Autenticado | Estadísticas de precios para una categoría. |
| GET | `/api/prendas/descuentos-por-categoria/{categoria}` | Autenticado | Descuentos disponibles para una categoría. |
| GET | `/api/prendas/busqueda` | Autenticado | Búsqueda por nombre, categoría y/o género. |
| GET | `/api/prendas/busqueda-por-nombre-genero` | Autenticado | Búsqueda por nombre y género. |
| GET | `/api/prendas/con-descuentos` | Autenticado | Prendas con descuento activo, filtrables. |
| GET | `/api/prendas/filtro` | Autenticado | Filtro dinámico por múltiples criterios. |

## Endpoints — Inventario (PrendaTallas)

| Método | Ruta | Autorización | Descripción |
|--------|------|-------------|-------------|
| GET | `/api/prendatallas` | Autenticado | Lista paginada de registros prenda-talla. |
| GET | `/api/prendatallas/{id}` | Autenticado | Registro prenda-talla por ID. |
| POST | `/api/prendatallas` | ADMIN | Asociar una talla a una prenda con stock. |
| PUT | `/api/prendatallas/{id}` | ADMIN | Actualizar registro prenda-talla. |
| DELETE | `/api/prendatallas/{id}` | ADMIN | Eliminar asociación prenda-talla. |
| PUT | `/api/prendatallas/stock/decremento` | ADMIN | Restar 1 unidad de stock. |
| PUT | `/api/prendatallas/stock/incremento` | ADMIN | Sumar 1 unidad de stock. |
| PUT | `/api/prendatallas/stock/suma` | ADMIN | Sumar N unidades de stock. |

---

## GET /api/prendas

Lista paginada de todas las prendas del catálogo.

### Query params

| Param | Tipo | Default | Descripción |
|-------|------|---------|-------------|
| `page` | `integer` | `1` | Número de página. |
| `pageSize` | `integer` | `10` | Registros por página. |

### Response `200 OK` — `ApiPaginationResponse<PrendaResponseDto>`

Ver [PrendaResponseDto](#prendaresponsedto).

---

## GET /api/prendas/{id}

### Path params

| Param | Tipo | Descripción |
|-------|------|-------------|
| `id` | `long` | ID de la prenda. |

### Response `200 OK` — `ApiResponse<PrendaResponseDto>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 404 | `NotFound` | La prenda no existe. |

---

## POST /api/prendas

Requiere rol `ADMIN`.

### Request body

```json
{
  "nombre": "Camiseta Básica",
  "descripcion": "Camiseta de algodón",
  "marcaId": 1,
  "categoriaId": 2,
  "proveedorId": 3,
  "generoId": 1,
  "precio": 29.99,
  "activo": true
}
```

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `nombre` | `string` | Sí | Nombre de la prenda. |
| `descripcion` | `string?` | No | Descripción opcional. |
| `marcaId` | `long` | Sí | ID de la marca. |
| `categoriaId` | `long` | Sí | ID de la categoría. |
| `proveedorId` | `long` | Sí | ID del proveedor. |
| `generoId` | `long` | Sí | ID del género. |
| `precio` | `decimal` | Sí | Precio base de la prenda. |
| `activo` | `boolean` | Sí | Si la prenda está publicada. |

### Response `201 Created` — `ApiResponse<PrendaResponseDto>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 400 | — | Campos inválidos (FluentValidation). |
| 404 | `NotFound` | Marca, categoría, proveedor o género no existen. |

---

## PUT /api/prendas/{id}

Requiere rol `ADMIN`. Mismos campos que POST. El `id` se toma de la ruta — no incluirlo en el body.

### Response `200 OK` — `ApiResponse<PrendaResponseDto>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 404 | `NotFound` | La prenda o alguna FK referenciada no existe. |

---

## DELETE /api/prendas/{id}

Requiere rol `ADMIN`.

### Response `200 OK`

```json
{ "success": true, "data": true }
```

---

## GET /api/prendas/tallas-por-genero/{genero}

Devuelve los nombres de talla disponibles en prendas activas del género dado.

### Response `200 OK` — `ApiResponse<string[]>`

```json
{ "success": true, "data": ["XS", "S", "M", "L", "XL"] }
```

---

## GET /api/prendas/marcas-por-genero/{genero}

### Response `200 OK` — `ApiResponse<string[]>`

```json
{ "success": true, "data": ["Nike", "Adidas", "Zara"] }
```

---

## GET /api/prendas/categorias-por-genero/{genero}

### Response `200 OK` — `ApiResponse<string[]>`

```json
{ "success": true, "data": ["Camisetas", "Pantalones", "Chaquetas"] }
```

---

## GET /api/prendas/precios-por-genero/{genero}

Estadísticas de precios (mín, promedio, máx) para prendas activas del género.

### Response `200 OK` — `ApiResponse<PrendaPrecioStatsDto>`

Ver [PrendaPrecioStatsDto](#prendapreciostatsdto).

---

## GET /api/prendas/descuentos-por-genero/{genero}

### Response `200 OK` — `ApiResponse<string[]>`

```json
{ "success": true, "data": ["10%", "20%", "30%"] }
```

---

## GET /api/prendas/con-descuentos-aleatorio/{genero}

Selección aleatoria de prendas con descuentos activos del género dado. Usado para secciones destacadas del frontend.

### Response `200 OK` — `ApiResponse<PrendaConDescuentoTodoResponseDto[]>`

Ver [PrendaConDescuentoTodoResponseDto](#prendacondescuentotodoresponsedto).

---

## GET /api/prendas/tallas-por-categoria/{categoria}

### Response `200 OK` — `ApiResponse<string[]>`

```json
{ "success": true, "data": ["S", "M", "L"] }
```

---

## GET /api/prendas/marcas-por-categoria/{categoria}

### Response `200 OK` — `ApiResponse<string[]>`

---

## GET /api/prendas/precios-por-categoria/{categoria}

### Response `200 OK` — `ApiResponse<PrendaPrecioStatsDto>`

Ver [PrendaPrecioStatsDto](#prendapreciostatsdto).

---

## GET /api/prendas/descuentos-por-categoria/{categoria}

### Response `200 OK` — `ApiResponse<string[]>`

---

## GET /api/prendas/busqueda

Búsqueda de prendas con descuento activo. Todos los parámetros son opcionales y combinables.

### Query params

| Param | Tipo | Descripción |
|-------|------|-------------|
| `nombre` | `string?` | Filtra por nombre (búsqueda parcial). |
| `categoria` | `string?` | Filtra por categoría. |
| `genero` | `string?` | Filtra por género. |

### Response `200 OK` — `ApiResponse<PrendaConDescuentoResponseDto[]>`

Ver [PrendaConDescuentoResponseDto](#prendacondescuentoresponsedto).

---

## GET /api/prendas/busqueda-por-nombre-genero

Mismo comportamiento y parámetros que `/busqueda`. Alias orientado a búsquedas por nombre dentro de un género específico.

### Query params

| Param | Tipo | Descripción |
|-------|------|-------------|
| `nombre` | `string?` | Filtra por nombre. |
| `categoria` | `string?` | Filtra por categoría. |
| `genero` | `string?` | Filtra por género. |

### Response `200 OK` — `ApiResponse<PrendaConDescuentoResponseDto[]>`

---

## GET /api/prendas/con-descuentos

Prendas que tienen descuento activo aplicado (por categoría o por género), filtrables.

### Query params

| Param | Tipo | Descripción |
|-------|------|-------------|
| `categoria` | `string?` | Filtra por categoría. |
| `genero` | `string?` | Filtra por género. |

### Response `200 OK` — `ApiResponse<PrendaConDescuentoResponseDto[]>`

---

## GET /api/prendas/filtro

Filtro dinámico para el catálogo. Todos los parámetros son opcionales y combinables.

### Query params

| Param | Tipo | Descripción |
|-------|------|-------------|
| `talla` | `string?` | Nombre de talla (ej. `"M"`). |
| `categoria` | `string?` | Nombre de categoría. |
| `marca` | `string?` | Nombre de marca. |
| `genero` | `string?` | Nombre de género. |
| `precioMin` | `double?` | Precio mínimo (≥ 0). |
| `precioMax` | `double?` | Precio máximo (≥ 0, ≥ `precioMin`). |
| `descMin` | `double?` | Descuento mínimo en % (0–100). |
| `descMax` | `double?` | Descuento máximo en % (0–100, ≥ `descMin`). |

### Response `200 OK` — `ApiResponse<PrendaConDescuentoResponseDto[]>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 400 | — | `precioMin > precioMax` o descuento fuera de rango 0–100. |

---

## GET /api/prendatallas

Lista paginada de todas las combinaciones prenda-talla con su stock.

### Query params

| Param | Tipo | Default |
|-------|------|---------|
| `page` | `integer` | `1` |
| `pageSize` | `integer` | `10` |

### Response `200 OK` — `ApiPaginationResponse<PrendaTallaResponseDto>`

Ver [PrendaTallaResponseDto](#prendatallaresponsedto).

---

## GET /api/prendatallas/{id}

### Response `200 OK` — `ApiResponse<PrendaTallaResponseDto>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 404 | `NotFound` | El registro no existe. |

---

## POST /api/prendatallas

Requiere rol `ADMIN`. Asocia una talla a una prenda con un stock inicial.

### Request body

```json
{
  "prendaId": 10,
  "tallaId": 3,
  "stock": 25
}
```

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `prendaId` | `long` | Sí | ID de la prenda. |
| `tallaId` | `long` | Sí | ID de la talla. |
| `stock` | `integer` | No | Stock inicial (default `0`). |

### Response `201 Created` — `ApiResponse<PrendaTallaResponseDto>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 404 | `NotFound` | La prenda o la talla no existen. |
| 409 | `Conflict` | Ya existe esa combinación prenda-talla. |

---

## PUT /api/prendatallas/{id}

Requiere rol `ADMIN`. Actualiza una combinación prenda-talla. Mismos campos que POST sin `stock` como requerido.

### Response `200 OK` — `ApiResponse<PrendaTallaResponseDto>`

---

## DELETE /api/prendatallas/{id}

Requiere rol `ADMIN`.

### Response `200 OK`

```json
{ "success": true, "data": true }
```

---

## PUT /api/prendatallas/stock/decremento

Requiere rol `ADMIN`. Resta exactamente 1 unidad de stock a la combinación dada.

### Query params

| Param | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `prendaId` | `long` | Sí | ID de la prenda. |
| `tallaId` | `long` | Sí | ID de la talla. |

### Response `200 OK` — `ApiResponse<PrendaTallaResponseDto>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 400 | `Validation` | Stock insuficiente (stock = 0) o combinación no encontrada. |

---

## PUT /api/prendatallas/stock/incremento

Requiere rol `ADMIN`. Suma exactamente 1 unidad de stock.

### Query params

| Param | Tipo | Requerido |
|-------|------|-----------|
| `prendaId` | `long` | Sí |
| `tallaId` | `long` | Sí |

### Response `200 OK` — `ApiResponse<PrendaTallaResponseDto>`

---

## PUT /api/prendatallas/stock/suma

Requiere rol `ADMIN`. Suma N unidades de stock.

### Query params

| Param | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `prendaId` | `long` | Sí | ID de la prenda. |
| `tallaId` | `long` | Sí | ID de la talla. |
| `cantidad` | `integer` | Sí | Unidades a sumar. |

### Response `200 OK` — `ApiResponse<PrendaTallaResponseDto>`

### Errores posibles

| HTTP | `error` | Cuándo |
|------|---------|--------|
| 404 | `NotFound` | Combinación prenda-talla no encontrada. |

---

## DTOs de respuesta

### PrendaResponseDto

Respuesta de operaciones CRUD básicas sobre prendas.

```json
{
  "id": 1,
  "nombre": "Camiseta Básica",
  "descripcion": "Camiseta de algodón",
  "marcaId": 1,
  "categoriaId": 2,
  "proveedorId": 3,
  "generoId": 1,
  "precio": 29.99,
  "activo": true,
  "createdAt": "2025-01-15T10:00:00Z",
  "updatedAt": "2025-06-01T08:30:00Z"
}
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | `long` | ID de la prenda. |
| `nombre` | `string` | Nombre de la prenda. |
| `descripcion` | `string?` | Descripción opcional. |
| `marcaId` | `long` | ID de la marca. |
| `categoriaId` | `long` | ID de la categoría. |
| `proveedorId` | `long` | ID del proveedor. |
| `generoId` | `long` | ID del género. |
| `precio` | `decimal` | Precio base. |
| `activo` | `boolean` | Si la prenda está publicada. |
| `createdAt` | `datetime?` | Fecha de creación (UTC). |
| `updatedAt` | `datetime?` | Última actualización (UTC). |

---

### PrendaConDescuentoResponseDto

Respuesta de endpoints de búsqueda y filtro con descuentos aplicados.

```json
{
  "id": 1,
  "nombre": "Camiseta Básica",
  "precio": 29.99,
  "imagenPrincipal": "https://...",
  "imagenHover": "https://...",
  "marca": "Nike",
  "descuentoAplicado": 15.00,
  "tipoDescuento": "CATEGORIA"
}
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | `long` | ID de la prenda. |
| `nombre` | `string` | Nombre de la prenda. |
| `precio` | `decimal` | Precio base (antes del descuento). |
| `imagenPrincipal` | `string?` | URL de la imagen principal. |
| `imagenHover` | `string?` | URL de la imagen al hacer hover. |
| `marca` | `string` | Nombre de la marca. |
| `descuentoAplicado` | `decimal` | Porcentaje de descuento aplicado. |
| `tipoDescuento` | `string` | Origen del descuento (`"CATEGORIA"`, `"PRENDA"`, etc.). |

---

### PrendaConDescuentoTodoResponseDto

Igual que `PrendaConDescuentoResponseDto` con el campo adicional `categoria`. Usado por el endpoint de descuentos aleatorios.

| Campo adicional | Tipo | Descripción |
|-----------------|------|-------------|
| `categoria` | `string` | Nombre de la categoría de la prenda. |

---

### PrendaPrecioStatsDto

Estadísticas de rango de precios de un conjunto de prendas.

```json
{
  "minimo": 9.99,
  "promedio": 34.50,
  "maximo": 89.99
}
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `minimo` | `decimal?` | Precio más bajo. |
| `promedio` | `decimal?` | Precio promedio. |
| `maximo` | `decimal?` | Precio más alto. |

---

### PrendaTallaResponseDto

Respuesta de operaciones sobre el inventario por talla.

```json
{
  "id": 5,
  "prendaId": 1,
  "tallaId": 3,
  "stock": 25,
  "createdAt": "2025-01-15T10:00:00Z",
  "updatedAt": "2025-06-01T08:30:00Z"
}
```

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | `long` | ID del registro prenda-talla. |
| `prendaId` | `long` | ID de la prenda. |
| `tallaId` | `long` | ID de la talla. |
| `stock` | `integer` | Unidades disponibles. |
| `createdAt` | `datetime?` | Fecha de creación (UTC). |
| `updatedAt` | `datetime?` | Última actualización (UTC). |

---

## Mapeo de rutas Java → .NET

El frontend anterior usaba un backend Spring con rutas distintas. Referencia de equivalencias:

| Java/Spring (anterior) | .NET MixAndMatch (actual) |
|------------------------|--------------------------|
| `GET /prendas/descuentos-aplicados-por-genero/{genero}` | `GET /api/prendas/con-descuentos-aleatorio/{genero}` |
| `GET /todas-prendas-filtradas?{params}` | `GET /api/prendas/filtro` |
| `GET /prendas/tallas-por-genero/{genero}` | `GET /api/prendas/tallas-por-genero/{genero}` |
| `GET /prendas/marcas-por-genero/{genero}` | `GET /api/prendas/marcas-por-genero/{genero}` |
| `GET /prendas/categorias-por-genero/{genero}` | `GET /api/prendas/categorias-por-genero/{genero}` |
| `GET /prendas/estadisticas-precios-por-genero/{genero}` | `GET /api/prendas/precios-por-genero/{genero}` |
| `GET /prendas/descuentos-por-genero/{genero}` | `GET /api/prendas/descuentos-por-genero/{genero}` |
| `GET /prendas/buscar-por-nombre-genero?{params}` | `GET /api/prendas/busqueda-por-nombre-genero` |
| `GET /prendas-filtradas?{params}` | `GET /api/prendas/filtro` |
| `GET /prenda-tallas/{categoria}` | `GET /api/prendas/tallas-por-categoria/{categoria}` |
| `GET /prenda-marcas/{categoria}` | `GET /api/prendas/marcas-por-categoria/{categoria}` |
| `GET /prenda-precios/{categoria}` | `GET /api/prendas/precios-por-categoria/{categoria}` |
| `GET /prendas/todos-descuentos/{categoria}` | `GET /api/prendas/descuentos-por-categoria/{categoria}` |
| `GET /prendas/buscar?{params}` | `GET /api/prendas/busqueda` |
| `GET /prenda/{id}` | `GET /api/prendas/{id}` |

> La respuesta del backend Spring usaba `d.object` como campo de datos. En esta API el campo equivalente es `data` dentro del envelope `ApiResponse<T>`. Ver [api-responses.md](./api-responses.md).
