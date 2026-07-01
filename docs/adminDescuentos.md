# Administración de Descuentos

Base URL: `http://localhost:5221/api`

Todos los endpoints requieren `Authorization: Bearer {token}`.

---

## Tipos de descuento

| Tipo | Descripción | Controller |
|------|-------------|------------|
| **Código** | Cupones de descuento que el cliente ingresa manualmente | `/api/DescuentoCodigos` |
| **Prenda** | Descuento aplicado a una prenda específica | `/api/DescuentoPrendas` |
| **Categoría** | Descuento aplicado a todas las prendas de una categoría | `/api/DescuentoCategorias` |
| **Uso de cupón** | Registro de qué usuario usó qué cupón | `/api/DescuentoUsuarios` |

---

## 1. Descuento por Código (Cupones)

### Response base

```json
{
  "id": 1,
  "codigo": "PROMO20",
  "descripcion": "20% de descuento en toda la tienda",
  "porcentaje": 20.0,
  "fechaInicio": "2026-01-01",
  "fechaFin": "2026-12-31",
  "usoMaximo": 100,
  "activo": true,
  "createdAt": "2026-06-30T00:00:00Z",
  "updatedAt": null
}
```

---

### GET /api/DescuentoCodigos
Listar todos los códigos paginado.

**Rol:** Cualquier autenticado  
**Query params:** `page` (default 1), `pageSize` (default 10)

```
GET /api/DescuentoCodigos?page=1&pageSize=10
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "items": [ ...DescuentoCodigoResponseDto ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 10
  }
}
```

---

### GET /api/DescuentoCodigos/{id}
Obtener cupón por ID.

**Rol:** Cualquier autenticado

```
GET /api/DescuentoCodigos/1
```

**Response 200:**
```json
{
  "success": true,
  "data": { ...DescuentoCodigoResponseDto }
}
```

**Response 404:**
```json
{ "success": false, "message": "Descuento de código no encontrado para id 1." }
```

---

### GET /api/DescuentoCodigos/codigo/{codigo}
Validar y obtener un cupón por su código (paso previo antes de aplicar).

**Rol:** CLIENTE

```
GET /api/DescuentoCodigos/codigo/PROMO20
```

**Response 200:**
```json
{
  "success": true,
  "data": { ...DescuentoCodigoResponseDto }
}
```

**Response 404:**
```json
{ "success": false, "message": "Código de descuento 'PROMO20' no encontrado." }
```

---

### POST /api/DescuentoCodigos/codigo/{codigo}/aplicar
Validar y registrar el uso del cupón por el usuario autenticado.  
Registra el uso en `DescuentoUsuario`. Si el cupón llega a `usoMaximo`, el job lo desactiva automáticamente.

**Rol:** CLIENTE  
**Body:** ninguno

```
POST /api/DescuentoCodigos/codigo/PROMO20/aplicar
```

**Response 200** — cupón aplicado, usar `porcentaje` para calcular el descuento:
```json
{
  "success": true,
  "data": {
    "id": 1,
    "codigo": "PROMO20",
    "descripcion": "20% de descuento en toda la tienda",
    "porcentaje": 20.0,
    "fechaInicio": "2026-01-01",
    "fechaFin": "2026-12-31",
    "usoMaximo": 100,
    "activo": true,
    "createdAt": "2026-06-30T00:00:00Z",
    "updatedAt": null
  }
}
```

**Response 400** — cupón inactivo o fuera de vigencia:
```json
{ "success": false, "message": "El código de descuento no está activo." }
{ "success": false, "message": "El código de descuento está fuera del período de vigencia." }
```

**Response 404** — código no existe:
```json
{ "success": false, "message": "El código 'PROMO20' no existe." }
```

**Response 409** — el usuario ya lo usó:
```json
{ "success": false, "message": "Ya has utilizado este código de descuento." }
```

---

### POST /api/DescuentoCodigos
Crear un nuevo cupón.

**Rol:** ADMIN

**Body:**
```json
{
  "codigo": "PROMO20",
  "descripcion": "20% de descuento en toda la tienda",
  "porcentaje": 20.0,
  "fechaInicio": "2026-01-01",
  "fechaFin": "2026-12-31",
  "usoMaximo": 100,
  "activo": true
}
```

| Campo | Tipo | Requerido | Notas |
|-------|------|-----------|-------|
| `codigo` | string | ✅ | Único en el sistema |
| `descripcion` | string | ❌ | Texto visible al cliente |
| `porcentaje` | decimal | ✅ | Ej: `20.0` = 20% |
| `fechaInicio` | DateOnly | ✅ | Formato `YYYY-MM-DD` |
| `fechaFin` | DateOnly | ❌ | `null` = sin vencimiento |
| `usoMaximo` | int | ✅ | Nro máximo de usos totales |
| `activo` | bool | ✅ | |

**Response 201:**
```json
{ "success": true, "data": { ...DescuentoCodigoResponseDto } }
```

**Response 409** — código duplicado:
```json
{ "success": false, "message": "El código de descuento ya existe." }
```

---

### PUT /api/DescuentoCodigos/{id}
Actualizar un cupón existente.

**Rol:** ADMIN

**Body:** mismo esquema que POST (todos los campos requeridos).

**Response 200:**
```json
{ "success": true, "data": { ...DescuentoCodigoResponseDto } }
```

---

### DELETE /api/DescuentoCodigos/{id}
Eliminar un cupón.

**Rol:** ADMIN

**Response 200:**
```json
{ "success": true, "data": null }
```

---

## 2. Descuento por Prenda

Aplica un porcentaje de descuento a una prenda específica. Los jobs de Hangfire sincronizan `activo` según las fechas automáticamente.

### Response base

```json
{
  "id": 1,
  "prendaId": 5,
  "porcentaje": 15.0,
  "fechaInicio": "2026-06-01",
  "fechaFin": "2026-06-30",
  "activo": true,
  "createdAt": "2026-06-01T00:00:00Z",
  "updatedAt": null
}
```

---

### GET /api/DescuentoPrendas
**Rol:** Cualquier autenticado | **Query:** `page`, `pageSize`

### GET /api/DescuentoPrendas/{id}
**Rol:** Cualquier autenticado

### POST /api/DescuentoPrendas
**Rol:** ADMIN

**Body:**
```json
{
  "prendaId": 5,
  "porcentaje": 15.0,
  "fechaInicio": "2026-06-01",
  "fechaFin": "2026-06-30",
  "activo": true
}
```

| Campo | Tipo | Requerido | Notas |
|-------|------|-----------|-------|
| `prendaId` | long | ✅ | Debe existir |
| `porcentaje` | decimal | ✅ | |
| `fechaInicio` | DateOnly | ✅ | |
| `fechaFin` | DateOnly | ❌ | |
| `activo` | bool | ✅ | |

**Response 201:** `{ "success": true, "data": { ...DescuentoPrendaResponseDto } }`

**Response 400** — prenda no existe:
```json
{ "success": false, "message": "Prenda no encontrada para id 5." }
```

### PUT /api/DescuentoPrendas/{id}
**Rol:** ADMIN | **Body:** mismo esquema que POST.

### DELETE /api/DescuentoPrendas/{id}
**Rol:** ADMIN

---

## 3. Descuento por Categoría

Aplica un porcentaje de descuento a todas las prendas de una categoría.

### Response base

```json
{
  "id": 1,
  "categoriaId": 3,
  "porcentaje": 10.0,
  "fechaInicio": "2026-07-01",
  "fechaFin": null,
  "activo": true,
  "createdAt": "2026-06-30T00:00:00Z",
  "updatedAt": null
}
```

---

### GET /api/DescuentoCategorias
**Rol:** Cualquier autenticado | **Query:** `page`, `pageSize`

### GET /api/DescuentoCategorias/{id}
**Rol:** Cualquier autenticado

### POST /api/DescuentoCategorias
**Rol:** ADMIN

**Body:**
```json
{
  "categoriaId": 3,
  "porcentaje": 10.0,
  "fechaInicio": "2026-07-01",
  "fechaFin": null,
  "activo": true
}
```

| Campo | Tipo | Requerido | Notas |
|-------|------|-----------|-------|
| `categoriaId` | long | ✅ | Debe existir |
| `porcentaje` | decimal | ✅ | |
| `fechaInicio` | DateOnly | ✅ | |
| `fechaFin` | DateOnly | ❌ | |
| `activo` | bool | ✅ | |

**Response 201:** `{ "success": true, "data": { ...DescuentoCategoriaResponseDto } }`

**Response 400** — categoría no existe:
```json
{ "success": false, "message": "Categoría no encontrada para id 3." }
```

### PUT /api/DescuentoCategorias/{id}
**Rol:** ADMIN | **Body:** mismo esquema que POST.

### DELETE /api/DescuentoCategorias/{id}
**Rol:** ADMIN

---

## 4. Historial de uso de cupones (DescuentoUsuarios)

Registra qué usuario usó qué cupón. Normalmente se crea vía `POST /codigo/{codigo}/aplicar` — estos endpoints son para consulta y gestión manual.

### Response base

```json
{
  "id": 1,
  "descuentoCodigoId": 1,
  "usuarioId": 42,
  "fechaUso": "2026-06-30",
  "createdAt": "2026-06-30T15:00:00Z",
  "updatedAt": null
}
```

---

### GET /api/DescuentoUsuarios
Historial de cupones usados por el usuario autenticado (paginado).

**Rol:** CLIENTE | **Query:** `page`, `pageSize`

**Response 200:**
```json
{
  "success": true,
  "data": {
    "items": [ ...DescuentoUsuarioResponseDto ],
    "totalCount": 3,
    "page": 1,
    "pageSize": 10
  }
}
```

---

### GET /api/DescuentoUsuarios/{id}
Obtener registro de uso por ID (solo el propio usuario).

**Rol:** CLIENTE

**Response 404** — no existe o no pertenece al usuario:
```json
{ "success": false, "message": "Registro no encontrado." }
```

---

### POST /api/DescuentoUsuarios
Registrar uso de cupón manualmente por ID (alternativa a `aplicar`).

**Rol:** CLIENTE

**Body:**
```json
{
  "descuentoCodigoId": 1,
  "fechaUso": "2026-06-30"
}
```

**Response 201:** `{ "success": true, "data": { ...DescuentoUsuarioResponseDto } }`

**Response 409** — ya usó este cupón:
```json
{ "success": false, "message": "Ya registraste el uso de este código de descuento." }
```

---

### DELETE /api/DescuentoUsuarios/{id}
Eliminar registro de uso (solo el propio usuario).

**Rol:** CLIENTE

---

## Resumen de roles

| Endpoint | ADMIN | CLIENTE |
|----------|-------|---------|
| `GET /DescuentoCodigos` | ✅ | ✅ |
| `GET /DescuentoCodigos/codigo/{codigo}` | — | ✅ |
| `POST /DescuentoCodigos/codigo/{codigo}/aplicar` | — | ✅ |
| `POST/PUT/DELETE /DescuentoCodigos` | ✅ | — |
| `GET /DescuentoPrendas` | ✅ | ✅ |
| `POST/PUT/DELETE /DescuentoPrendas` | ✅ | — |
| `GET /DescuentoCategorias` | ✅ | ✅ |
| `POST/PUT/DELETE /DescuentoCategorias` | ✅ | — |
| `GET/POST/DELETE /DescuentoUsuarios` | — | ✅ (solo propios) |

---

## Jobs automáticos (Hangfire)

| Job | Frecuencia | Acción |
|-----|------------|--------|
| `SincronizarEstadosDescuentosJob` | Periódico | Activa descuentos cuya `fechaInicio` llegó; desactiva los que pasaron `fechaFin` |
| `ExpirarCodigosPorUsoJob` | Periódico | Desactiva cupones donde `COUNT(usos) >= usoMaximo` |
