# Módulo Carrito

Gestiona los carritos de compra (`Carrito`) y sus ítems (`CarritoItem`). Todos los endpoints devuelven el formato descrito en [api-responses.md](./api-responses.md).

**Base URL Carrito:** `/api/carritos`
**Base URL CarritoItem:** `/api/carrito-items`

---

## Endpoints — Carrito

| Método | Ruta                                          | Autenticación       | Descripción                                                          |
|--------|-----------------------------------------------|---------------------|----------------------------------------------------------------------|
| GET    | `/api/carritos`                               | ADMIN               | Lista paginada de todos los carritos.                                |
| GET    | `/api/carritos/{id}`                          | CLIENTE             | Carrito por ID. Solo el dueño puede verlo.                           |
| POST   | `/api/carritos`                               | CLIENTE             | Crea un nuevo carrito para el usuario autenticado.                   |
| PUT    | `/api/carritos/{id}`                          | CLIENTE             | Actualiza el estado del carrito.                                     |
| DELETE | `/api/carritos/{id}`                          | CLIENTE             | Elimina un carrito propio.                                           |
| GET    | `/api/carritos/abierto/usuario/{usuarioId}`   | ADMIN + CLIENTE     | Carritos activos de un usuario. CLIENTE siempre ve los propios.      |
| GET    | `/api/carritos/{id}/cantidad-items`           | ADMIN + CLIENTE     | Cantidad de ítems distintos en un carrito.                           |

---

## Endpoints — CarritoItem

| Método | Ruta                                   | Autenticación | Descripción                                                                                   |
|--------|----------------------------------------|---------------|-----------------------------------------------------------------------------------------------|
| GET    | `/api/carrito-items`                   | ADMIN         | Lista paginada de todos los ítems de carrito.                                                 |
| GET    | `/api/carrito-items/{id}`              | CLIENTE       | Ítem por ID. Solo el dueño del carrito puede verlo.                                           |
| POST   | `/api/carrito-items`                   | CLIENTE       | Agrega un ítem al carrito (requiere `prendaTallaId` ya conocido).                             |
| PUT    | `/api/carrito-items/{id}`              | CLIENTE       | Actualiza precio y cantidad de un ítem propio.                                                |
| DELETE | `/api/carrito-items/{id}`              | CLIENTE       | Elimina un ítem del carrito.                                                                  |
| POST   | `/api/carrito-items/agregar`           | CLIENTE       | Agrega o incrementa un ítem por `prendaId + tallaId`. Resuelve `prendaTallaId` internamente. |
| PUT    | `/api/carrito-items/{id}/cantidad`     | CLIENTE       | Actualiza solo la cantidad de un ítem propio.                                                 |

---

## GET /api/carritos  *(solo ADMIN)*

Lista paginada de todos los carritos del sistema.

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
      "id": 3,
      "usuarioId": 42,
      "fechaCreacion": "2025-06-20T10:00:00Z",
      "estado": "ACTIVO",
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

## GET /api/carritos/{id}  *(CLIENTE)*

Devuelve un carrito por ID. Solo el dueño puede verlo — si el carrito pertenece a otro usuario devuelve 403.

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 3,
    "usuarioId": 42,
    "fechaCreacion": "2025-06-20T10:00:00Z",
    "estado": "ACTIVO",
    "updatedAt": null
  },
  "error": null
}
```

### Errores posibles

| HTTP | `error`     | Cuándo                                     |
|------|-------------|--------------------------------------------|
| 404  | `NotFound`  | No existe un carrito con ese ID.           |
| 403  | `Forbidden` | El carrito pertenece a otro usuario.       |

---

## DELETE /api/carritos/{id}  *(CLIENTE)*

Elimina un carrito propio. Si el carrito no pertenece al usuario autenticado devuelve 403.

### Response `200 OK`

```json
{
  "success": true,
  "message": "Carrito eliminado correctamente.",
  "data": null,
  "error": null
}
```

### Errores posibles

| HTTP | `error`     | Cuándo                                     |
|------|-------------|--------------------------------------------|
| 404  | `NotFound`  | No existe un carrito con ese ID.           |
| 403  | `Forbidden` | El carrito pertenece a otro usuario.       |

---

## GET /api/carritos/abierto/usuario/{usuarioId}

Devuelve los carritos con estado `ACTIVO` de un usuario.

- **ADMIN**: puede pasar cualquier `usuarioId` en la ruta.
- **CLIENTE**: el `usuarioId` de la ruta se ignora — siempre se devuelven los carritos del usuario autenticado.

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 3,
      "usuarioId": 42,
      "fechaCreacion": "2025-06-20T10:00:00Z",
      "estado": "ACTIVO",
      "updatedAt": null
    }
  ],
  "error": null
}
```

---

## GET /api/carritos/{id}/cantidad-items

Devuelve cuántos ítems distintos hay en un carrito.

- **CLIENTE**: solo puede consultar carritos propios (403 si el carrito no le pertenece).
- **ADMIN**: puede consultar cualquier carrito.

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": 4,
  "error": null
}
```

### Errores posibles

| HTTP | `error`     | Cuándo                                     |
|------|-------------|--------------------------------------------|
| 404  | `NotFound`  | No existe un carrito con ese ID.           |
| 403  | `Forbidden` | El carrito pertenece a otro usuario.       |

---

## POST /api/carritos

Crea un carrito vacío para el usuario autenticado. El `usuarioId` se toma del JWT — no se envía body.

### Response `201 Created`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 5,
    "usuarioId": 42,
    "fechaCreacion": "2025-06-28T12:00:00Z",
    "estado": "ACTIVO",
    "updatedAt": null
  },
  "error": null,
  "status": "Created"
}
```

---

## PUT /api/carritos/{id}

Actualiza el estado de un carrito propio.

### Request body

```json
"COMPLETADO"
```

> El body es un `string` directamente (no un objeto JSON). Valores válidos: `ACTIVO`, `ABANDONADO`, `COMPLETADO`.

---

## GET /api/carrito-items  *(solo ADMIN)*

Lista paginada de todos los ítems de carrito del sistema.

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
      "id": 22,
      "carritoId": 3,
      "prendaTallaId": 17,
      "precioUnitario": 89.90,
      "cantidad": 2,
      "createdAt": "2025-06-28T12:05:00Z",
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

## GET /api/carrito-items/{id}  *(CLIENTE)*

Devuelve un ítem de carrito por ID. Verifica que el carrito al que pertenece el ítem sea del usuario autenticado.

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 22,
    "carritoId": 3,
    "prendaTallaId": 17,
    "precioUnitario": 89.90,
    "cantidad": 2,
    "createdAt": "2025-06-28T12:05:00Z",
    "updatedAt": null
  },
  "error": null
}
```

### Errores posibles

| HTTP | `error`     | Cuándo                                                        |
|------|-------------|---------------------------------------------------------------|
| 404  | `NotFound`  | No existe un ítem con ese ID.                                 |
| 403  | `Forbidden` | El carrito del ítem pertenece a otro usuario.                 |

---

## DELETE /api/carrito-items/{id}  *(CLIENTE)*

Elimina un ítem del carrito. Verifica que el carrito al que pertenece el ítem sea del usuario autenticado.

### Response `200 OK`

```json
{
  "success": true,
  "message": "CarritoItem eliminado correctamente.",
  "data": null,
  "error": null
}
```

### Errores posibles

| HTTP | `error`     | Cuándo                                                        |
|------|-------------|---------------------------------------------------------------|
| 404  | `NotFound`  | No existe un ítem con ese ID.                                 |
| 403  | `Forbidden` | El carrito del ítem pertenece a otro usuario.                 |

---

## POST /api/carrito-items/agregar

Agrega o incrementa un ítem en el carrito buscando por `prendaId + tallaId`. Si el ítem ya existe, suma 1 a la cantidad. Si no existe, lo crea con precio tomado de `Prenda.Precio` y `cantidad = 1`.

### Query params

| Param       | Tipo   | Descripción                       |
|-------------|--------|-----------------------------------|
| `carritoId` | `long` | ID del carrito destino.           |
| `prendaId`  | `long` | ID de la prenda.                  |
| `tallaId`   | `long` | ID de la talla.                   |

### Errores posibles

| HTTP | `error`     | Cuándo                                                          |
|------|-------------|-----------------------------------------------------------------|
| 403  | `Forbidden` | El carrito no pertenece al usuario autenticado.                 |
| 404  | `NotFound`  | No existe la combinación prenda-talla o la prenda no existe.    |
| 409  | `Conflict`  | El carrito no está en estado `ACTIVO`.                          |
| 409  | `Conflict`  | Stock disponible es 0.                                          |

---

## PUT /api/carrito-items/{id}/cantidad

Actualiza solo la cantidad de un ítem propio, sin modificar el `precioUnitario`.

### Query params

| Param      | Tipo  | Descripción              |
|------------|-------|--------------------------|
| `cantidad` | `int` | Nueva cantidad (> 0).    |

### Errores posibles

| HTTP | `error`     | Cuándo                                               |
|------|-------------|------------------------------------------------------|
| 400  | —           | `cantidad` es 0 o negativa.                          |
| 403  | `Forbidden` | El carrito del ítem no pertenece al solicitante.     |
| 404  | `NotFound`  | No existe un ítem con ese ID.                        |

---

## POST /api/carrito-items

Agrega un ítem a un carrito activo cuando ya se conoce el `prendaTallaId`. El `solicitanteId` se toma del JWT.

### Request body

```json
{
  "carritoId": 3,
  "prendaTallaId": 17,
  "precioUnitario": 89.90,
  "cantidad": 2
}
```

| Campo           | Tipo      | Requerido | Descripción                                           |
|-----------------|-----------|-----------|-------------------------------------------------------|
| `carritoId`     | `long`    | Sí        | ID del carrito al que se agrega el ítem.              |
| `prendaTallaId` | `long`    | Sí        | ID de la combinación prenda-talla.                    |
| `precioUnitario`| `decimal` | Sí        | Precio al momento de agregar (snapshot).              |
| `cantidad`      | `int`     | Sí        | Cantidad de unidades.                                 |

> `solicitanteId` **no se envía** — lo asigna el controller desde el JWT.

### Response `201 Created`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 22,
    "carritoId": 3,
    "prendaTallaId": 17,
    "precioUnitario": 89.90,
    "cantidad": 2,
    "createdAt": "2025-06-28T12:05:00Z",
    "updatedAt": null
  },
  "error": null,
  "status": "Created"
}
```

### Errores posibles

| HTTP | `error`     | Cuándo                                                        |
|------|-------------|---------------------------------------------------------------|
| 400  | —           | Campos inválidos.                                             |
| 409  | `Conflict`  | La prenda-talla ya está en el carrito.                        |
| 409  | `Conflict`  | El carrito no está en estado `ACTIVO`.                        |
| 403  | `Forbidden` | El carrito no pertenece al usuario autenticado.               |
| 404  | `NotFound`  | El carrito o la prenda-talla no existen.                      |

---

## PUT /api/carrito-items/{id}

Actualiza precio unitario y cantidad de un ítem propio.

### Request body

```json
{
  "precioUnitario": 79.90,
  "cantidad": 3
}
```

> `carritoItemId` y `solicitanteId` **no se envían** — vienen de la ruta y del JWT respectivamente.

---

## DTOs

### CarritoResponseDto

```json
{
  "id": 3,
  "usuarioId": 42,
  "fechaCreacion": "2025-06-20T10:00:00Z",
  "estado": "ACTIVO",
  "updatedAt": null
}
```

| Campo           | Tipo        | Descripción                                          |
|-----------------|-------------|------------------------------------------------------|
| `id`            | `long`      | ID interno del carrito.                              |
| `usuarioId`     | `long`      | Dueño del carrito.                                   |
| `fechaCreacion` | `datetime?` | Fecha de creación.                                   |
| `estado`        | `string?`   | `ACTIVO` · `ABANDONADO` · `COMPLETADO`               |
| `updatedAt`     | `datetime?` |                                                      |

---

### CarritoItemResponseDto

```json
{
  "id": 22,
  "carritoId": 3,
  "prendaTallaId": 17,
  "precioUnitario": 89.90,
  "cantidad": 2,
  "createdAt": "2025-06-28T12:05:00Z",
  "updatedAt": null
}
```

| Campo           | Tipo        | Descripción                                          |
|-----------------|-------------|------------------------------------------------------|
| `id`            | `long`      | ID interno del ítem.                                 |
| `carritoId`     | `long`      | Carrito al que pertenece.                            |
| `prendaTallaId` | `long`      | Combinación prenda-talla seleccionada.               |
| `precioUnitario`| `decimal`   | Precio en el momento de agregar.                     |
| `cantidad`      | `int`       | Unidades.                                            |
| `createdAt`     | `datetime?` |                                                      |
| `updatedAt`     | `datetime?` |                                                      |

---

## Migración desde el frontend antiguo

| Función JS                         | Endpoint JS (Spring)                               | .NET nuevo                                                    | Cambios clave                                                                                                   |
|------------------------------------|----------------------------------------------------|---------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| `getCarritoAbierto(usuarioId)`     | `GET /carrito/abierto/usuario/{id}`                | `GET /api/carritos/abierto/usuario/{usuarioId}`               | CLIENTE puede consultarlo. El `usuarioId` de la ruta se ignora — siempre devuelve los carritos propios.         |
| `getCarrito(carritoId)`            | `GET /carrito/{id}`                                | `GET /api/carritos/{id}`                                      | Sin cambios relevantes.                                                                                         |
| `getCantidadItems(carritoId)`      | `GET /carrito/{id}/cantidad-items`                 | `GET /api/carritos/{id}/cantidad-items`                       | CLIENTE puede consultarlo. Antes solo ADMIN.                                                                    |
| `createCarrito(usuarioId)`         | `POST /carrito` con `{ usuarioId, estado }`        | `POST /api/carritos` sin body                                 | El `usuarioId` se toma del JWT — no enviar nada en el body.                                                    |
| `updateCarrito` / `actualizarCarrito` | `PUT /carrito/{id}` con `{ usuarioId, estado }` | `PUT /api/carritos/{id}` con body `"COMPLETADO"`              | El body es solo el string del estado, no un objeto.                                                             |
| `agregarCarritoItem(carritoId, prendaId, tallaId)` | `POST /carrito-item/agregar` (query params) | `POST /api/carrito-items/agregar?carritoId=&prendaId=&tallaId=` | Ruta cambiada de `carrito-item` a `carrito-items`. Mismo comportamiento: agrega o incrementa.              |
| `createCarritoItem(carritoId, prendaId, talla, cantidad, precio)` | `POST /carrito-item` con `prendaId + talla` | `POST /api/carrito-items` con `prendaTallaId`   | El frontend viejo enviaba `prendaId` y `talla` por separado. El nuevo necesita el `prendaTallaId`. Preferir `agregar`. |
| `updateItemCantidad(itemId, cantidad)` | `PUT /carrito-item/{id}/cantidad?cantidad=`    | `PUT /api/carrito-items/{id}/cantidad?cantidad=`              | Ruta cambiada de `carrito-item` a `carrito-items`. Mismo comportamiento.                                        |
| `deleteCarritoItem(itemId)`        | `DELETE /carrito-item/{id}`                        | `DELETE /api/carrito-items/{id}`                              | Ruta cambiada de `carrito-item` a `carrito-items`.                                                              |
| `sumarUno(prendaId, tallaId)`      | `PUT /sumar-uno?prendaId=&tallaId=`                | `PUT /api/prenda-tallas/stock/incremento`                     | Son de **stock de inventario**, no de carrito. Solo ADMIN.                                                      |
| `restarUno(prendaId, tallaId)`     | `PUT /restar-uno?prendaId=&tallaId=`               | `PUT /api/prenda-tallas/stock/decremento`                     | Ídem — solo ADMIN.                                                                                              |
| `sumarStock(prendaId, tallaId, cantidad)` | `PUT /sumar?prendaId=&tallaId=&cantidad=`    | `PUT /api/prenda-tallas/stock/suma`                           | Ídem — solo ADMIN.                                                                                              |
| `aplicarCupon(usuarioId, codigo)`  | `POST /aplicar` con `{ usuarioId, codigo }`        | `GET /api/descuento-codigos/codigo/{codigo}` → `POST /api/descuento-usuarios` | Ahora son 2 llamadas: primero se resuelve el ID por código, luego se registra el uso. |
| `createCarritoDetalle(ventaId, carritoId)` | `POST /carritodetalle`                      | `POST /api/ventas/carrito-detalle`                            | Sigue siendo una llamada explícita del frontend. Movida de `carritoApi` a `ventaApi`. Ver [ventas.md](./ventas.md). |

---

## Cliente JS actualizado

Reemplaza el archivo `carritoApi.js` completo con el siguiente código:

```javascript
import axios from "axios";
import { API_BASE } from "../../../config/api";

const authHeaders = () => ({
  Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
});

// ─── Carrito ────────────────────────────────────────────────────────────────

export const getCarritoAbierto = (usuarioId) =>
  axios
    .get(`${API_BASE}/carritos/abierto/usuario/${usuarioId}`, { headers: authHeaders() })
    .then((r) => r.data.data);

export const getCarrito = (carritoId) =>
  axios
    .get(`${API_BASE}/carritos/${carritoId}`, { headers: authHeaders() })
    .then((r) => r.data.data);

export const getCantidadItems = (carritoId) =>
  axios
    .get(`${API_BASE}/carritos/${carritoId}/cantidad-items`, { headers: authHeaders() })
    .then((r) => r.data.data);

// userId viene del JWT — no enviar body
export const createCarrito = () =>
  axios
    .post(`${API_BASE}/carritos`, null, { headers: authHeaders() })
    .then((r) => r.data.data);

// body es solo el string del estado
export const updateCarrito = (carritoId, estado = "COMPLETADO") =>
  axios.put(`${API_BASE}/carritos/${carritoId}`, JSON.stringify(estado), {
    headers: { ...authHeaders(), "Content-Type": "application/json" },
  });

export const actualizarCarrito = (carritoId) => updateCarrito(carritoId, "COMPLETADO");

// ─── CarritoItem ────────────────────────────────────────────────────────────

// Agrega o incrementa por prendaId + tallaId (resuelve prendaTallaId internamente)
export const agregarCarritoItem = (carritoId, prendaId, tallaId) =>
  axios.post(`${API_BASE}/carrito-items/agregar`, null, {
    params: { carritoId, prendaId, tallaId },
    headers: authHeaders(),
  });

// Solo usar si ya se tiene prendaTallaId — de lo contrario preferir agregarCarritoItem
export const createCarritoItem = (carritoId, prendaTallaId, cantidad, precioUnitario) =>
  axios.post(
    `${API_BASE}/carrito-items`,
    { carritoId, prendaTallaId, cantidad, precioUnitario },
    { headers: authHeaders() }
  );

export const updateItemCantidad = (itemId, cantidad) =>
  axios.put(`${API_BASE}/carrito-items/${itemId}/cantidad`, null, {
    params: { cantidad },
    headers: authHeaders(),
  });

export const deleteCarritoItem = (itemId) =>
  axios.delete(`${API_BASE}/carrito-items/${itemId}`, { headers: authHeaders() });

// ─── Stock (solo ADMIN) ─────────────────────────────────────────────────────

export const sumarUno = (prendaId, tallaId) =>
  axios
    .put(`${API_BASE}/prenda-tallas/stock/incremento`, null, {
      params: { prendaId, tallaId },
      headers: authHeaders(),
    })
    .then((r) => r.data);

export const restarUno = (prendaId, tallaId) =>
  axios
    .put(`${API_BASE}/prenda-tallas/stock/decremento`, null, {
      params: { prendaId, tallaId },
      headers: authHeaders(),
    })
    .then((r) => r.data);

export const sumarStock = (prendaId, tallaId, cantidad) =>
  axios.put(`${API_BASE}/prenda-tallas/stock/suma`, null, {
    params: { prendaId, tallaId, cantidad },
    headers: authHeaders(),
  });

// ─── Cupones ────────────────────────────────────────────────────────────────

// Busca el descuento por su código de texto y registra el uso en 2 pasos
export const aplicarCupon = async (codigo) => {
  const descuento = await axios
    .get(`${API_BASE}/descuento-codigos/codigo/${codigo}`, { headers: authHeaders() })
    .then((r) => r.data.data);
  return axios.post(
    `${API_BASE}/descuento-usuarios`,
    { descuentoCodigoId: descuento.id },
    { headers: authHeaders() }
  );
};
```
