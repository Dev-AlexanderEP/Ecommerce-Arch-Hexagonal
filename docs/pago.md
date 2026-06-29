# Módulo Pago

Gestiona los métodos de pago disponibles (`MetodoPago`) y los pagos registrados por cada venta (`Pago`). Todos los endpoints devuelven el formato descrito en [api-responses.md](./api-responses.md).

**Base URL MetodoPago:** `/api/metodo-pago`
**Base URL Pago:** `/api/pago`

---

## Endpoints — MetodoPago

| Método | Ruta                      | Autenticación    | Descripción                          |
|--------|---------------------------|------------------|--------------------------------------|
| GET    | `/api/metodo-pago`        | Bearer JWT       | Lista paginada de métodos de pago.   |
| GET    | `/api/metodo-pago/{id}`   | Bearer JWT       | Método de pago por ID.               |
| POST   | `/api/metodo-pago`        | ADMIN            | Crea un nuevo método de pago.        |
| PUT    | `/api/metodo-pago/{id}`   | ADMIN            | Actualiza un método de pago.         |
| DELETE | `/api/metodo-pago/{id}`   | ADMIN            | Elimina un método de pago.           |

> `GET` es accesible por cualquier usuario autenticado (CLIENTE y ADMIN) — el frontend lo usa para poblar el selector de método de pago en el checkout.

---

## Endpoints — Pago

| Método | Ruta              | Autenticación       | Descripción                                      |
|--------|-------------------|---------------------|--------------------------------------------------|
| GET    | `/api/pago`       | ADMIN               | Lista paginada de todos los pagos.               |
| GET    | `/api/pago/{id}`  | ADMIN / CLIENTE     | Pago por ID. CLIENTE solo puede ver el propio.   |
| POST   | `/api/pago`       | CLIENTE             | Registra el pago de una venta propia.            |
| PUT    | `/api/pago/{id}`  | ADMIN               | Actualiza estado y datos de un pago.             |
| DELETE | `/api/pago/{id}`  | ADMIN               | Elimina un pago.                                 |

---

## GET /api/metodo-pago

Devuelve la lista paginada de métodos disponibles. El frontend la usa para mostrar las opciones al cliente en el checkout.

### Query params

| Param      | Tipo  | Default | Descripción      |
|------------|-------|---------|------------------|
| `page`     | `int` | `1`     | Página actual.   |
| `pageSize` | `int` | `10`    | Resultados/página.|

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": [
    { "id": 1, "tipoPago": "Tarjeta de crédito", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": null },
    { "id": 2, "tipoPago": "Yape",               "createdAt": "2025-01-01T00:00:00Z", "updatedAt": null }
  ],
  "metadata": {
    "totalCount": 2,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasNext": false,
    "hasPrev": false
  },
  "error": null
}
```

---

## POST /api/pago

Registra el pago de una venta. Solo el dueño de la venta puede pagarla. El monto **no se envía en el body** — el servidor lo calcula desde el total de los ítems de la venta.

### Request body

```json
{
  "ventaId": 12,
  "metodoId": 1
}
```

| Campo     | Tipo   | Requerido | Descripción                                      |
|-----------|--------|-----------|--------------------------------------------------|
| `ventaId` | `long` | Sí        | ID de la venta a pagar.                          |
| `metodoId`| `long` | Sí        | ID del método de pago seleccionado.              |

> **`monto` y `estado` no se envían.** El monto se calcula desde `venta.Total` (suma de `VentasDetalle`). El estado siempre comienza como `PENDIENTE`; el ADMIN lo actualiza a `COMPLETADO` o `FALLIDO` después.

### Response `201 Created`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 7,
    "ventaId": 12,
    "metodoId": 1,
    "monto": 149.90,
    "estado": "PENDIENTE",
    "fechaCreacion": "2025-06-28T15:30:00Z",
    "updatedAt": null
  },
  "error": null,
  "status": "Created"
}
```

### Errores posibles

| HTTP | `error`      | Cuándo                                                              |
|------|--------------|---------------------------------------------------------------------|
| 400  | —            | Campos inválidos (FluentValidation).                                |
| 400  | `Validation` | La venta no existe.                                                 |
| 400  | `Validation` | El método de pago no existe.                                        |
| 400  | `Validation` | La venta no tiene ítems — `Total = 0`, no hay nada que pagar.      |
| 403  | `Forbidden`  | La venta pertenece a otro usuario.                                  |

---

## PUT /api/pago/{id}  *(solo ADMIN)*

Permite al ADMIN actualizar el estado de un pago (p. ej. marcar como `COMPLETADO` tras confirmar el cobro, o `FALLIDO`/`REEMBOLSADO`).

### Request body

```json
{
  "ventaId": 12,
  "metodoId": 1,
  "monto": 149.90,
  "estado": "COMPLETADO"
}
```

| Campo     | Tipo      | Requerido | Descripción                                                            |
|-----------|-----------|-----------|------------------------------------------------------------------------|
| `ventaId` | `long`    | Sí        |                                                                        |
| `metodoId`| `long`    | Sí        |                                                                        |
| `monto`   | `decimal` | Sí        | El ADMIN puede corregir el monto si es necesario.                      |
| `estado`  | `string`  | Sí        | Uno de: `PENDIENTE`, `COMPLETADO`, `FALLIDO`, `REEMBOLSADO`.          |

### Errores posibles

| HTTP | `error`      | Cuándo                                              |
|------|--------------|-----------------------------------------------------|
| 400  | `Validation` | La venta o el método de pago no existen.            |
| 400  | `Validation` | `estado` no es un valor válido del enum.            |
| 404  | `NotFound`   | No existe un pago con ese ID.                       |

---

## DTOs

### MetodoPagoResponseDto

```json
{
  "id": 1,
  "tipoPago": "Yape",
  "createdAt": "2025-01-01T00:00:00Z",
  "updatedAt": null
}
```

| Campo       | Tipo        | Descripción             |
|-------------|-------------|-------------------------|
| `id`        | `long`      | ID interno.             |
| `tipoPago`  | `string`    | Nombre del método.      |
| `createdAt` | `datetime?` |                         |
| `updatedAt` | `datetime?` |                         |

---

### PagoResponseDto

```json
{
  "id": 7,
  "ventaId": 12,
  "metodoId": 1,
  "monto": 149.90,
  "estado": "COMPLETADO",
  "fechaCreacion": "2025-06-28T15:30:00Z",
  "updatedAt": "2025-06-29T10:00:00Z"
}
```

| Campo          | Tipo        | Descripción                                                  |
|----------------|-------------|--------------------------------------------------------------|
| `id`           | `long`      | ID interno.                                                  |
| `ventaId`      | `long`      | ID de la venta asociada.                                     |
| `metodoId`     | `long`      | ID del método de pago usado.                                 |
| `monto`        | `decimal`   | Monto calculado desde `venta.Total`.                         |
| `estado`       | `string`    | `PENDIENTE` · `COMPLETADO` · `FALLIDO` · `REEMBOLSADO`      |
| `fechaCreacion`| `datetime`  |                                                              |
| `updatedAt`    | `datetime?` |                                                              |

---

## Migración desde el frontend antiguo

| Endpoint JS (Spring)        | .NET nuevo              | Cambios clave                                                                         |
|-----------------------------|-------------------------|---------------------------------------------------------------------------------------|
| `GET /metodo-pagos`         | `GET /api/metodo-pago`  | Nombre en singular. Mismo acceso (cualquier usuario autenticado).                     |
| `POST /pago` con `{ ventaId, monto, metodoId, estado: "PAGADO" }` | `POST /api/pago` con `{ ventaId, metodoId }` | `monto` y `estado` los calcula el servidor — no los envíes. |
