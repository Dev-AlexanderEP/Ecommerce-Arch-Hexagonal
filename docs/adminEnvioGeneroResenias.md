# Envíos, Géneros y Reseñas

Base URL: `http://localhost:5221/api`

Todos los endpoints requieren `Authorization: Bearer {token}`.

---

## 1. Envíos `/api/Envio`

Gestión del envío asociado a una venta. Cada venta puede tener un solo envío.

### Response base

```json
{
  "id": 1,
  "ventaId": 10,
  "datosEnvioId": 3,
  "costoEnvio": 0.00,
  "fechaEnvio": "2026-06-30",
  "fechaEntrega": "2026-07-05",
  "estado": "EN_TRANSITO",
  "metodoEnvio": "Delivery",
  "trackingNumber": "TRK-00123",
  "createdAt": "2026-06-30T10:00:00Z",
  "updatedAt": null
}
```

**Estados válidos:** `PENDIENTE`, `EN_TRANSITO`, `ENTREGADO`, `CANCELADO`

---

### GET /api/Envio
Listar todos los envíos paginado.

**Rol:** ADMIN | **Query:** `page` (default 1), `pageSize` (default 10)

**Response 200:**
```json
{
  "success": true,
  "data": {
    "items": [ ...EnvioResponseDto ],
    "totalCount": 50,
    "page": 1,
    "pageSize": 10
  }
}
```

---

### GET /api/Envio/{id}
Obtener envío por ID.

**Rol:** ADMIN

**Response 404:**
```json
{ "success": false, "message": "Envío no encontrado para id 1." }
```

---

### GET /api/Envio/tracking/{trackingNumber}
Buscar un envío por número de seguimiento.

**Rol:** CLIENTE

```
GET /api/Envio/tracking/TRK-00123
```

---

### GET /api/Envio/usuario/{userId}/entregados
Envíos entregados de un usuario específico.

**Rol:** ADMIN

**Response 200:**
```json
{ "success": true, "data": [ ...EnvioResponseDto ] }
```

---

### GET /api/Envio/usuario/{userId}/no-entregados
Envíos pendientes/en tránsito de un usuario específico.

**Rol:** ADMIN

---

### POST /api/Envio
Crear el envío de una venta. Una venta solo puede tener un envío — si ya existe devuelve 409.

**Rol:** ADMIN o CLIENTE (el cliente solo puede crear para sus propias ventas)

**Body:**
```json
{
  "ventaId": 10,
  "datosEnvioId": 3,
  "costoEnvio": 0.00,
  "fechaEnvio": "2026-06-30",
  "fechaEntrega": "2026-07-05",
  "estado": "PENDIENTE",
  "metodoEnvio": "Delivery",
  "trackingNumber": "TRK-00123"
}
```

| Campo | Tipo | Requerido | Notas |
|-------|------|-----------|-------|
| `ventaId` | long | ✅ | Debe existir |
| `datosEnvioId` | long | ✅ | Debe existir |
| `costoEnvio` | decimal | ✅ | |
| `fechaEnvio` | DateOnly | ✅ | Formato `YYYY-MM-DD` |
| `fechaEntrega` | DateOnly | ❌ | |
| `estado` | string | ✅ | `PENDIENTE`, `EN_TRANSITO`, `ENTREGADO`, `CANCELADO` |
| `metodoEnvio` | string | ✅ | |
| `trackingNumber` | string | ❌ | |

**Response 201:** `{ "success": true, "data": { ...EnvioResponseDto } }`

**Response 409** — venta ya tiene envío:
```json
{ "success": false, "message": "La venta 10 ya tiene un envío registrado." }
```

**Response 403** — cliente intentando crear envío de otra persona:
```json
{ "success": false, "message": "No tienes acceso a esta venta." }
```

---

### PUT /api/Envio/{id}
Actualizar un envío (ej. cambiar estado a `EN_TRANSITO` o agregar `trackingNumber`).

**Rol:** ADMIN | **Body:** mismo esquema que POST.

**Response 200:** `{ "success": true, "data": { ...EnvioResponseDto } }`

---

### DELETE /api/Envio/{id}
Eliminar un envío.

**Rol:** ADMIN

**Response 200:** `{ "success": true, "data": null }`

---

## 2. Géneros `/api/Generos`

Catálogo de géneros de prendas (Hombre, Mujer, Unisex, etc.).

### Response base

```json
{
  "id": 1,
  "nomGenero": "Mujer"
}
```

---

### GET /api/Generos
**Rol:** Cualquier autenticado | **Query:** `page`, `pageSize`

**Response 200:**
```json
{
  "success": true,
  "data": {
    "items": [ { "id": 1, "nomGenero": "Mujer" } ],
    "totalCount": 3,
    "page": 1,
    "pageSize": 10
  }
}
```

---

### GET /api/Generos/{id}
**Rol:** Cualquier autenticado

**Response 404:**
```json
{ "success": false, "message": "Género no encontrado para id 1." }
```

---

### POST /api/Generos
**Rol:** ADMIN

**Body:**
```json
{ "nomGenero": "Unisex" }
```

**Response 201:** `{ "success": true, "data": { "id": 3, "nomGenero": "Unisex" } }`

**Response 409** — nombre duplicado:
```json
{ "success": false, "message": "El género ya existe." }
```

---

### PUT /api/Generos/{id}
**Rol:** ADMIN

**Body:** `{ "nomGenero": "Unisex" }`

**Response 200:** `{ "success": true, "data": { ...GeneroResponseDto } }`

---

### DELETE /api/Generos/{id}
**Rol:** ADMIN

**Response 200:** `{ "success": true, "data": null }`

---

## 3. Reseñas `/api/Resenias`

Los clientes crean reseñas de prendas. El admin las modera (aprueba o rechaza).

### Response base

```json
{
  "id": 1,
  "prendaId": 5,
  "usuarioId": 42,
  "calificacion": 5,
  "comentario": "Excelente calidad, muy cómodo.",
  "estado": "PENDIENTE",
  "moderadoPorId": null,
  "moderadoEn": null,
  "motivoRechazo": null,
  "createdAt": "2026-06-30T10:00:00Z",
  "updatedAt": "2026-06-30T10:00:00Z"
}
```

**Estados:** `PENDIENTE`, `APROBADA`, `RECHAZADA`

---

### GET /api/Resenias
Buscar reseñas con filtros opcionales.

**Rol:** ADMIN o CLIENTE  
**Query params:**

| Param | Tipo | Descripción |
|-------|------|-------------|
| `prendaId` | long | Filtrar por prenda |
| `usuarioId` | long | Filtrar por usuario |
| `calificacion` | int | Filtrar por puntuación (1-5) |
| `page` | int | Default 1 |
| `pageSize` | int | Default 10, máx 100 |

```
GET /api/Resenias?prendaId=5&calificacion=5&page=1&pageSize=10
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "items": [ ...ReseniaResponseDto ],
    "totalCount": 8,
    "page": 1,
    "pageSize": 10
  }
}
```

---

### GET /api/Resenias/prenda/{prendaId}
Todas las reseñas de una prenda específica.

**Rol:** ADMIN o CLIENTE

**Response 200:** `{ "success": true, "data": [ ...ReseniaResponseDto ] }`

---

### GET /api/Resenias/{id}
**Rol:** Cualquier autenticado

**Response 404:**
```json
{ "success": false, "message": "Resenia no encontrada para id 1." }
```

---

### POST /api/Resenias
Crear una reseña. Un cliente solo puede reseñar una vez por prenda. La reseña nace en estado `PENDIENTE`.

**Rol:** CLIENTE

**Body:**
```json
{
  "prendaId": 5,
  "calificacion": 5,
  "comentario": "Excelente calidad, muy cómodo."
}
```

| Campo | Tipo | Requerido | Notas |
|-------|------|-----------|-------|
| `prendaId` | long | ✅ | Debe existir |
| `calificacion` | int | ✅ | 1 a 5 |
| `comentario` | string | ❌ | |

**Response 201:** `{ "success": true, "data": { ...ReseniaResponseDto } }`

**Response 409** — ya tiene reseña para esa prenda:
```json
{ "success": false, "message": "Ya tienes una resenia para esta prenda." }
```

---

### PUT /api/Resenias/{id}
Editar calificación o comentario. Solo si la reseña está en estado `PENDIENTE`.

**Rol:** CLIENTE (solo las propias)

**Body:**
```json
{
  "calificacion": 4,
  "comentario": "Buena calidad."
}
```

**Response 403** — reseña de otro usuario:
```json
{ "success": false, "message": "No tienes acceso a esta resenia." }
```

**Response 409** — ya fue moderada:
```json
{ "success": false, "message": "Solo se pueden actualizar resenias en estado PENDIENTE." }
```

---

### DELETE /api/Resenias/{id}
Eliminar una reseña.

**Rol:** ADMIN (cualquiera) o CLIENTE (solo las propias)

**Response 200:** `{ "success": true, "data": null }`

---

### PATCH /api/Resenias/{id}/estado
Moderar una reseña: aprobarla o rechazarla. Solo disponible para el admin.

**Rol:** ADMIN

**Body:**
```json
{
  "estado": "RECHAZADA",
  "motivoRechazo": "Contenido inapropiado."
}
```

| Campo | Tipo | Requerido | Notas |
|-------|------|-----------|-------|
| `estado` | string | ✅ | `APROBADA` o `RECHAZADA` |
| `motivoRechazo` | string | ❌ | Requerido si `estado = RECHAZADA` |

**Response 200:** `{ "success": true, "data": { ...ReseniaResponseDto } }`

---

## Resumen de roles

| Endpoint | ADMIN | CLIENTE |
|----------|-------|---------|
| `GET /Envio` (lista) | ✅ | — |
| `GET /Envio/{id}` | ✅ | — |
| `GET /Envio/tracking/{n}` | — | ✅ |
| `GET /Envio/usuario/{id}/entregados` | ✅ | — |
| `GET /Envio/usuario/{id}/no-entregados` | ✅ | — |
| `POST /Envio` | ✅ | ✅ (propias ventas) |
| `PUT/DELETE /Envio` | ✅ | — |
| `GET /Generos` | ✅ | ✅ |
| `POST/PUT/DELETE /Generos` | ✅ | — |
| `GET /Resenias` | ✅ | ✅ |
| `POST /Resenias` | — | ✅ |
| `PUT /Resenias/{id}` | — | ✅ (propias, PENDIENTE) |
| `DELETE /Resenias/{id}` | ✅ | ✅ (propias) |
| `PATCH /Resenias/{id}/estado` | ✅ | — |
