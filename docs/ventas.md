# Módulo Ventas

Gestiona las órdenes de compra (`Venta`) y sus líneas de detalle (`VentasDetalle`). Todos los endpoints devuelven el formato descrito en [api-responses.md](./api-responses.md).

**Base URL Venta:** `/api/ventas`
**Base URL VentasDetalle:** `/api/ventas-detalles`

---

## Endpoints — Venta

| Método | Ruta                                      | Autenticación       | Descripción                                                           |
|--------|-------------------------------------------|---------------------|-----------------------------------------------------------------------|
| GET    | `/api/ventas`                             | ADMIN               | Lista paginada de todas las ventas.                                   |
| GET    | `/api/ventas/{id}`                        | ADMIN + CLIENTE     | Venta por ID. CLIENTE solo puede ver las propias.                     |
| POST   | `/api/ventas`                             | CLIENTE             | Crea una nueva venta en estado `PENDIENTE`.                           |
| PUT    | `/api/ventas/{id}`                        | ADMIN               | Actualiza el estado de una venta.                                     |
| DELETE | `/api/ventas/{id}`                        | ADMIN               | Elimina una venta.                                                    |
| GET    | `/api/ventas/segunda-pendiente`           | CLIENTE             | Devuelve el ID de la segunda venta pendiente del usuario autenticado. |
| POST   | `/api/ventas/carrito-detalle`             | CLIENTE             | Copia los ítems de un carrito como `VentasDetalle` de una venta.     |

---

## Endpoints — VentasDetalle

| Método | Ruta                         | Autenticación   | Descripción                                        |
|--------|------------------------------|-----------------|----------------------------------------------------|
| GET    | `/api/ventas-detalles`       | ADMIN           | Lista paginada de todos los detalles.              |
| GET    | `/api/ventas-detalles/{id}`  | ADMIN + CLIENTE | Detalle por ID. CLIENTE solo ve los propios.       |
| POST   | `/api/ventas-detalles`       | ADMIN           | Crea un detalle manualmente.                       |
| PUT    | `/api/ventas-detalles/{id}`  | ADMIN           | Actualiza un detalle.                              |
| DELETE | `/api/ventas-detalles/{id}`  | ADMIN           | Elimina un detalle.                                |

---

## Flujo de checkout

El frontend realiza estas llamadas en orden al completar una compra:

```
1. POST /api/ventas               → crea la Venta (PENDIENTE), recibe ventaId
2. POST /api/ventas/carrito-detalle { ventaId, carritoId }
                                  → copia CarritoItems como VentasDetalle
3. POST /api/pago { ventaId, metodoId }
                                  → registra el pago (PENDIENTE)
4. PUT  /api/carritos/{id}  body: "COMPLETADO"
                                  → marca el carrito como completado
```

---

## GET /api/ventas  *(solo ADMIN)*

Lista paginada de todas las ventas del sistema.

### Query params

| Param      | Tipo  | Default | Descripción       |
|------------|-------|---------|-------------------|
| `page`     | `int` | `1`     | Página actual.    |
| `pageSize` | `int` | `10`    | Resultados/página.|

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 12,
      "usuarioId": 42,
      "fechaCreacion": "2025-06-28T15:00:00Z",
      "estado": "PENDIENTE",
      "updatedAt": null
    }
  ],
  "metadata": {
    "totalCount": 1,
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

## GET /api/ventas/{id}  *(ADMIN + CLIENTE)*

Devuelve una venta por ID. El CLIENTE solo puede ver sus propias ventas (403 si la venta pertenece a otro usuario).

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 12,
    "usuarioId": 42,
    "fechaCreacion": "2025-06-28T15:00:00Z",
    "estado": "PENDIENTE",
    "updatedAt": null
  },
  "error": null
}
```

### Errores posibles

| HTTP | `error`     | Cuándo                                        |
|------|-------------|-----------------------------------------------|
| 404  | `NotFound`  | No existe una venta con ese ID.               |
| 403  | `Forbidden` | La venta pertenece a otro usuario.            |

---

## POST /api/ventas  *(CLIENTE)*

Crea una nueva venta en estado `PENDIENTE`. El `usuarioId` se toma del JWT — no se envía body.

Además dispara un email de confirmación de forma asíncrona (no bloquea la respuesta).

### Response `201 Created`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 12,
    "usuarioId": 42,
    "fechaCreacion": "2025-06-28T15:00:00Z",
    "estado": "PENDIENTE",
    "updatedAt": null
  },
  "error": null,
  "status": "Created"
}
```

---

## PUT /api/ventas/{id}  *(solo ADMIN)*

Actualiza el estado de una venta.

### Request body

```json
{
  "estado": "PAGADO"
}
```

| Campo    | Tipo     | Requerido | Descripción                                                              |
|----------|----------|-----------|--------------------------------------------------------------------------|
| `estado` | `string` | Sí        | Uno de: `PENDIENTE`, `PAGADO`, `ENVIADO`, `ENTREGADO`, `CANCELADO`.     |

### Errores posibles

| HTTP | `error`      | Cuándo                                      |
|------|--------------|---------------------------------------------|
| 400  | `Validation` | `estado` no es un valor válido del enum.    |
| 404  | `NotFound`   | No existe una venta con ese ID.             |

---

## GET /api/ventas/segunda-pendiente  *(CLIENTE)*

Devuelve el ID de la segunda venta en estado `PENDIENTE` del usuario autenticado, ordenadas por `fechaCreacion` ascendente. Se usa en el flujo de checkout para localizar la venta recién creada que será pagada.

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": 12,
  "error": null
}
```

### Errores posibles

| HTTP | `error`    | Cuándo                                                        |
|------|------------|---------------------------------------------------------------|
| 404  | `NotFound` | El usuario no tiene una segunda venta en estado `PENDIENTE`.  |

---

## POST /api/ventas/carrito-detalle  *(CLIENTE)*

Copia todos los `CarritoItem` de un carrito como `VentasDetalle` de la venta indicada. Paso 2 del flujo de checkout — se llama justo después de `POST /api/ventas`.

### Request body

```json
{
  "ventaId": 12,
  "carritoId": 3
}
```

| Campo       | Tipo   | Requerido | Descripción                                      |
|-------------|--------|-----------|--------------------------------------------------|
| `ventaId`   | `long` | Sí        | ID de la venta donde se agregarán los detalles.  |
| `carritoId` | `long` | Sí        | ID del carrito cuyos ítems se van a copiar.      |

> `solicitanteId` **no se envía** — el controller lo asigna desde el JWT. El handler verifica que tanto la venta como el carrito pertenezcan al usuario autenticado.

### Response `201 Created`

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 31,
      "ventaId": 12,
      "prendaTallaId": 17,
      "cantidad": 2,
      "precioUnitario": 89.90,
      "createdAt": "2025-06-28T15:05:00Z",
      "updatedAt": null
    }
  ],
  "error": null,
  "status": "Created"
}
```

### Errores posibles

| HTTP | `error`      | Cuándo                                                |
|------|--------------|-------------------------------------------------------|
| 403  | `Forbidden`  | La venta o el carrito pertenecen a otro usuario.      |
| 404  | `NotFound`   | La venta o el carrito no existen.                     |
| 400  | `Validation` | El carrito no tiene ítems.                            |

---

## DTOs

### VentaResponseDto

```json
{
  "id": 12,
  "usuarioId": 42,
  "fechaCreacion": "2025-06-28T15:00:00Z",
  "estado": "PENDIENTE",
  "updatedAt": null
}
```

| Campo           | Tipo       | Descripción                                                          |
|-----------------|------------|----------------------------------------------------------------------|
| `id`            | `long`     | ID interno de la venta.                                              |
| `usuarioId`     | `long`     | Usuario dueño de la venta.                                           |
| `fechaCreacion` | `datetime` | Fecha de creación.                                                   |
| `estado`        | `string?`  | `PENDIENTE` · `PAGADO` · `ENVIADO` · `ENTREGADO` · `CANCELADO`      |
| `updatedAt`     | `datetime?`|                                                                      |

---

### VentasDetalleResponseDto

```json
{
  "id": 31,
  "ventaId": 12,
  "prendaTallaId": 17,
  "cantidad": 2,
  "precioUnitario": 89.90,
  "createdAt": "2025-06-28T15:05:00Z",
  "updatedAt": null
}
```

| Campo           | Tipo        | Descripción                              |
|-----------------|-------------|------------------------------------------|
| `id`            | `long`      | ID interno del detalle.                  |
| `ventaId`       | `long`      | Venta a la que pertenece.                |
| `prendaTallaId` | `long`      | Combinación prenda-talla.                |
| `cantidad`      | `int`       | Unidades compradas.                      |
| `precioUnitario`| `decimal`   | Precio al momento de la compra.          |
| `createdAt`     | `datetime?` |                                          |
| `updatedAt`     | `datetime?` |                                          |

---

## Migración desde el frontend antiguo

| Función JS                    | Endpoint JS (Spring)                               | .NET nuevo                            | Cambios clave                                                                                   |
|-------------------------------|----------------------------------------------------|---------------------------------------|-------------------------------------------------------------------------------------------------|
| `createVenta(usuarioId)`      | `POST /venta` con `{ usuarioId, estado }`          | `POST /api/ventas` sin body           | `usuarioId` viene del JWT. `estado` siempre es `PENDIENTE` — no enviar.                        |
| `getSegundaPendiente(usuarioId)` | `GET /venta/segunda-pendiente/{usuarioId}`      | `GET /api/ventas/segunda-pendiente`   | Sin param en la ruta — el `usuarioId` viene del JWT. Devuelve `data: <id>` (número).           |
| `deleteVenta(ventaId)`        | `DELETE /venta/{id}`                               | `DELETE /api/ventas/{id}`             | Ahora solo ADMIN.                                                                               |
| `updateVenta(ventaId, usuarioId)` | `PUT /venta/{id}` con `{ id, usuarioId, estado: "PAGADO" }` | `PUT /api/ventas/{id}` con `{ estado: "PAGADO" }` | Solo ADMIN. Body solo lleva `estado` — no enviar `id` ni `usuarioId`. |
| `createCarritoDetalle(ventaId, carritoId)` | `POST /carritodetalle`              | `POST /api/ventas/carrito-detalle`    | Movida de `carritoApi` a `ventaApi`. Ahora CLIENTE. `solicitanteId` del JWT.                   |

---

## Cliente JS actualizado

```javascript
import axios from "axios";
import { API_BASE } from "../../../config/api";

const authHeaders = () => ({
  Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
});

// userId viene del JWT — no enviar body
export const createVenta = () =>
  axios
    .post(`${API_BASE}/ventas`, null, { headers: authHeaders() })
    .then((r) => r.data.data);

export const getVenta = (ventaId) =>
  axios
    .get(`${API_BASE}/ventas/${ventaId}`, { headers: authHeaders() })
    .then((r) => r.data.data);

// Devuelve el ID (número) de la segunda venta pendiente del usuario
export const getSegundaPendiente = () =>
  axios
    .get(`${API_BASE}/ventas/segunda-pendiente`, { headers: authHeaders() })
    .then((r) => r.data.data);

// Solo ADMIN
export const deleteVenta = (ventaId) =>
  axios.delete(`${API_BASE}/ventas/${ventaId}`, { headers: authHeaders() });

// Solo ADMIN — body: solo el estado
export const updateVenta = (ventaId, estado) =>
  axios.put(
    `${API_BASE}/ventas/${ventaId}`,
    { estado },
    { headers: authHeaders() }
  );

// Paso 2 del checkout — copia CarritoItems como VentasDetalle
export const agregarDetallesDesdeCarrito = (ventaId, carritoId) =>
  axios
    .post(
      `${API_BASE}/ventas/carrito-detalle`,
      { ventaId, carritoId },
      { headers: authHeaders() }
    )
    .then((r) => r.data.data);

// Flujo completo de checkout
export const actualizarVentaPagada = async (ventaId, estado = "PAGADO") => {
  await updateVenta(ventaId, estado);
};
```
