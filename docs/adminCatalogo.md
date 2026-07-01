# Admin Catálogo — Endpoints

Cubre los módulos de catálogo básico: **Marcas**, **Categorías**, **Tallas**, **Proveedores** y **Géneros**.

Todos los endpoints requieren `Authorization: Bearer <token>`.  
Todos los responses siguen el envelope:
```json
{ "success": true, "message": "...", "data": { }, "errorType": null }
```

---

## MARCAS

**Response `data` común** — `MarcaResponseDto`:
```json
{ "id": 1, "nomMarca": "Nike" }
```

### 1. Listar marcas paginado

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | `/marcas/paginado` | `/api/marcas` |
| Auth | Bearer | Bearer |

**Query params**
```
page     integer   default: 1
pageSize integer   default: 10
```

**Response `data`**
```json
{
  "items": [
    { "id": 1, "nomMarca": "Nike" },
    { "id": 2, "nomMarca": "Adidas" }
  ],
  "totalCount": 8,
  "page": 1,
  "pageSize": 10
}
```

### 2. Obtener marca por id

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | *(no existía)* | `/api/marcas/{id}` |
| Auth | Bearer | Bearer |

**Response `data`** — `MarcaResponseDto` (ver arriba).

### 3. Crear marca

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/marca` | `/api/marcas` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomMarca": "Puma" }
```

**Validaciones**
- `409` si ya existe una marca con ese nombre

**Response `data`** — `MarcaResponseDto` con el nuevo id.

### 4. Actualizar marca

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | `/marca/{id}` | `/api/marcas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomMarca": "Puma Pro" }
```

**Validaciones**
- `404` si el id no existe
- `409` si ya existe otra marca con ese nombre

**Response `data`** — `MarcaResponseDto` actualizado.

### 5. Eliminar marca

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | `/marca/{id}` | `/api/marcas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request** — sin body, `id` en la ruta.

**Response `data`**
```json
true
```

---

## CATEGORÍAS

**Response `data` común** — `CategoriaResponseDto`:
```json
{ "id": 1, "nomCategoria": "Polos" }
```

### 1. Listar categorías paginado

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | `/categorias/paginado` | `/api/categorias` |
| Auth | Bearer | Bearer |

**Query params** — idénticos a Marcas (`page`, `pageSize`).

**Response `data`**
```json
{
  "items": [
    { "id": 1, "nomCategoria": "Polos" },
    { "id": 2, "nomCategoria": "Casacas" }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 10
}
```

### 2. Obtener categoría por id

| | Versión actual |
|---|---|
| Método | `GET` |
| Ruta | `/api/categorias/{id}` |
| Auth | Bearer |

**Response `data`** — `CategoriaResponseDto`.

### 3. Crear categoría

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/categoria` | `/api/categorias` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomCategoria": "Shorts" }
```

**Validaciones**
- `409` si ya existe una categoría con ese nombre

**Response `data`** — `CategoriaResponseDto` con el nuevo id.

### 4. Actualizar categoría

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | `/categoria/{id}` | `/api/categorias/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomCategoria": "Shorts Deportivos" }
```

**Validaciones**
- `404` si el id no existe
- `409` si ya existe otra categoría con ese nombre

**Response `data`** — `CategoriaResponseDto` actualizado.

### 5. Eliminar categoría

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | `/categoria/{id}` | `/api/categorias/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Response `data`**
```json
true
```

---

## TALLAS

**Response `data` común** — `TallaResponseDto`:
```json
{ "id": 1, "nomTalla": "M" }
```

### 1. Listar tallas paginado

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | `/tallas/paginado` | `/api/tallas` |
| Auth | Bearer | Bearer |

**Response `data`**
```json
{
  "items": [
    { "id": 1, "nomTalla": "S" },
    { "id": 2, "nomTalla": "M" },
    { "id": 3, "nomTalla": "L" }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 10
}
```

### 2. Obtener talla por id

| | Versión actual |
|---|---|
| Método | `GET` |
| Ruta | `/api/tallas/{id}` |
| Auth | Bearer |

### 3. Obtener talla por nombre (NUEVO)

> No existía en la versión antigua.

| | Versión actual |
|---|---|
| Método | `GET` |
| Ruta | `/api/tallas/por-nombre/{nomTalla}` |
| Auth | Bearer |

**Ejemplo**
```
GET /api/tallas/por-nombre/XL
```

**Response `data`** — `TallaResponseDto`.

### 4. Crear talla

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/talla` | `/api/tallas` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomTalla": "XL" }
```

**Validaciones**
- `409` si ya existe una talla con ese nombre

**Response `data`** — `TallaResponseDto` con el nuevo id.

### 5. Actualizar talla

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | `/talla/{id}` | `/api/tallas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomTalla": "XXL" }
```

**Validaciones**
- `404` si el id no existe
- `409` si ya existe otra talla con ese nombre

### 6. Eliminar talla

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | `/talla/{id}` | `/api/tallas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Response `data`**
```json
true
```

---

## PROVEEDORES

**Response `data` común** — `ProveedorResponseDto`:
```json
{ "id": 1, "nomProveedor": "Proveedor SA" }
```

### 1. Listar proveedores paginado

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | `/proveedores/paginado` | `/api/proveedores` |
| Auth | Bearer | Bearer |

**Response `data`**
```json
{
  "items": [
    { "id": 1, "nomProveedor": "Proveedor SA" }
  ],
  "totalCount": 3,
  "page": 1,
  "pageSize": 10
}
```

### 2. Obtener proveedor por id

| | Versión actual |
|---|---|
| Método | `GET` |
| Ruta | `/api/proveedores/{id}` |
| Auth | Bearer |

### 3. Crear proveedor

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/proveedor` | `/api/proveedores` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomProveedor": "Textiles Norte" }
```

**Validaciones**
- `409` si ya existe un proveedor con ese nombre

**Response `data`** — `ProveedorResponseDto` con el nuevo id.

### 4. Actualizar proveedor

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | `/proveedor/{id}` | `/api/proveedores/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{ "nomProveedor": "Textiles Norte SAC" }
```

**Validaciones**
- `404` si el id no existe
- `409` si ya existe otro proveedor con ese nombre

### 5. Eliminar proveedor

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | `/proveedor/{id}` | `/api/proveedores/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Response `data`**
```json
true
```

---

## GÉNEROS

**Response `data` común** — `GeneroResponseDto`:
```json
{ "id": 1, "nomGenero": "Hombre" }
```

> En `adminPrendas.md` sección 10 ya está documentado el `GET /api/generos` paginado. Esta sección completa el CRUD completo.

### 1. Listar géneros paginado

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | `/generos` | `/api/generos` |
| Auth | Bearer | Bearer |

**Response `data`**
```json
{
  "items": [
    { "id": 1, "nomGenero": "Hombre" },
    { "id": 2, "nomGenero": "Mujer" },
    { "id": 3, "nomGenero": "Unisex" }
  ],
  "totalCount": 3,
  "page": 1,
  "pageSize": 10
}
```

### 2. Obtener género por id

| | Versión actual |
|---|---|
| Método | `GET` |
| Ruta | `/api/generos/{id}` |
| Auth | Bearer |

### 3. Crear género

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | *(no existía)* | `/api/generos` |
| Auth | — | Bearer (ADMIN) |

**Request body**
```json
{ "nomGenero": "Niños" }
```

**Validaciones**
- `409` si ya existe un género con ese nombre

**Response `data`** — `GeneroResponseDto` con el nuevo id.

### 4. Actualizar género

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | *(no existía)* | `/api/generos/{id}` |
| Auth | — | Bearer (ADMIN) |

**Request body**
```json
{ "nomGenero": "Infantil" }
```

**Validaciones**
- `404` si el id no existe
- `409` si ya existe otro género con ese nombre

### 5. Eliminar género

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | *(no existía)* | `/api/generos/{id}` |
| Auth | — | Bearer (ADMIN) |

**Response `data`**
```json
true
```
