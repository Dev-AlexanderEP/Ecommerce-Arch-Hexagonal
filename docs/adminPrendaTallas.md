# Admin Prenda Tallas — Endpoints

Todos los endpoints requieren `Authorization: Bearer <token>`.  
Todos los responses siguen el envelope:
```json
{ "success": true, "message": "...", "data": { }, "errorType": null }
```

**Response `data` común** — `PrendaTallaResponseDto`:
```json
{
  "id": 10,
  "prendaId": 1,
  "tallaId": 3,
  "stock": 15,
  "createdAt": "2025-01-01T00:00:00",
  "updatedAt": null
}
```

---

## 1. Crear prenda-talla

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/prenda-talla` | `/api/prendaTallas` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{
  "prendaId": 1,
  "tallaId": 3,
  "stock": 15
}
```

**Validaciones**
- `404` si `prendaId` no existe
- `404` si `tallaId` no existe
- `409` si ya existe la combinación `prendaId + tallaId`

**Response `data`** — `PrendaTallaResponseDto` (ver arriba).

---

## 2. Actualizar prenda-talla

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | `/prenda-talla/{id}` | `/api/prendaTallas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

> Solo actualiza el `stock`. `prendaId` y `tallaId` no se pueden cambiar.

**Request body**
```json
{
  "stock": 20
}
```

**Response `data`** — `PrendaTallaResponseDto` con el stock actualizado.

---

## 3. Eliminar prenda-talla

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | `/prenda-talla/{id}` | `/api/prendaTallas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request** — sin body, `id` en la ruta.

**Response `data`**
```json
true
```

---

## 4. Decrementar stock en 1

> No existía en la versión antigua. Se usa al confirmar una compra.

| | Versión actual |
|---|---|
| Método | `PUT` |
| Ruta | `/api/prendaTallas/stock/decremento` |
| Auth | Bearer (ADMIN o CLIENTE) |

**Query params**
```
prendaId   long   requerido
tallaId    long   requerido
```

**Ejemplo**
```
PUT /api/prendaTallas/stock/decremento?prendaId=1&tallaId=3
```

**Validaciones**
- `400` si el stock es 0 o la combinación no existe

**Response `data`** — `PrendaTallaResponseDto` con el stock decrementado.

---

## 5. Incrementar stock en 1

> No existía en la versión antigua. Se usa al cancelar una compra o devolver al carrito.

| | Versión actual |
|---|---|
| Método | `PUT` |
| Ruta | `/api/prendaTallas/stock/incremento` |
| Auth | Bearer (ADMIN o CLIENTE) |

**Query params**
```
prendaId   long   requerido
tallaId    long   requerido
```

**Ejemplo**
```
PUT /api/prendaTallas/stock/incremento?prendaId=1&tallaId=3
```

**Response `data`** — `PrendaTallaResponseDto` con el stock incrementado.

---

## 6. Sumar cantidad arbitraria al stock

> No existía en la versión antigua. Se usa para reposición de inventario.

| | Versión actual |
|---|---|
| Método | `PUT` |
| Ruta | `/api/prendaTallas/stock/suma` |
| Auth | Bearer (ADMIN o CLIENTE) |

**Query params**
```
prendaId   long    requerido
tallaId    long    requerido
cantidad   integer requerido
```

**Ejemplo**
```
PUT /api/prendaTallas/stock/suma?prendaId=1&tallaId=3&cantidad=50
```

**Response `data`** — `PrendaTallaResponseDto` con el nuevo stock total.
