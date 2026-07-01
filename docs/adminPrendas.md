# Admin Prendas — Endpoints

Todos los endpoints requieren `Authorization: Bearer <token>` (rol ADMIN salvo indicación).  
Todos los responses siguen el envelope:
```json
{ "success": true, "message": "...", "data": { }, "errorType": null }
```

### Almacenamiento de imágenes — sistema actual (Cloudflare R2)

Las imágenes ya no se guardan en disco. Se almacenan en el bucket `mixandmatch-images` de R2 con la siguiente estructura de keys:

```
prendas/{prendaId}/{uuid}.{ext}

Ejemplo:
  prendas/42/550e8400-e29b-41d4-a716.jpg   ← PRINCIPAL
  prendas/42/7c9e6679-7425-40de-944b.webp  ← SECUNDARIA
  prendas/42/promo.mp4                      ← DETALLE (video)
```

El tipo de imagen (`PRINCIPAL`, `SECUNDARIA`, `DETALLE`) se guarda en BD, no en el nombre del archivo.  
La URL pública base es `https://pub-3a2696fb3f694eb790c98786a32a79ad.r2.dev`.

### Operaciones del sistema antiguo y su equivalente actual

| Sistema antiguo | Sistema actual |
|---|---|
| `uploadArchivo(formData)` → `POST /archivos/upload` | `POST /api/prendas/{id}/imagenes` (multipart) |
| `createImagen(data)` → `POST /imagen` | `POST /api/prendaImagenes` (URL directa) |
| `updateImagen(id, data)` → `PUT /imagen/{id}` | `PUT /api/prendaImagenes/{id}` |
| `eliminarCarpeta(ruta)` → `DELETE /archivos/eliminar-carpeta` | **Automático** al hacer `DELETE /api/prendas/{id}` — el handler borra todas las imágenes de R2 con prefijo `prendas/{id}/` antes de eliminar la prenda |
| `renombrarCarpeta(base, viejo, nuevo)` → `PUT /archivos/renombrar-carpeta` | **No aplica** — las keys usan el `id` numérico de la prenda, no el nombre; renombrar la prenda no afecta R2 |

---

## 1. Listar prendas paginado

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | `/prendas/paginado` | `/api/prendas` |
| Auth | Bearer | Bearer |

**Query params actuales**
```
page     integer   default: 1
pageSize integer   default: 10
```

**Response `data`**
```json
{
  "items": [
    {
      "id": 1,
      "nombre": "Polo Oversize",
      "descripcion": "...",
      "marcaId": 2,
      "categoriaId": 3,
      "proveedorId": 1,
      "generoId": 2,
      "precio": 89.90,
      "activo": true,
      "createdAt": "2025-01-01T00:00:00",
      "updatedAt": null
    }
  ],
  "totalCount": 42
}
```

---

## 2. Crear prenda

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/prenda` | `/api/prendas` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{
  "nombre": "Polo Oversize",
  "descripcion": "Descripción opcional",
  "marcaId": 2,
  "categoriaId": 3,
  "proveedorId": 1,
  "generoId": 2,
  "precio": 89.90,
  "activo": true
}
```

**Response `data`** — mismo shape que un item de la lista (ver sección 1).

---

## 3. Actualizar prenda

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | `/prenda/{id}` | `/api/prendas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body** — idéntico al de crear (todos los campos requeridos):
```json
{
  "nombre": "Polo Oversize v2",
  "descripcion": "...",
  "marcaId": 2,
  "categoriaId": 3,
  "proveedorId": 1,
  "generoId": 2,
  "precio": 99.90,
  "activo": true
}
```

> En el sistema antiguo, si el nombre cambiaba el frontend llamaba `renombrarCarpeta` para sincronizar la carpeta física. **Eso ya no es necesario** — los archivos en R2 viven bajo `prendas/{id}/`, el nombre de la prenda no forma parte del path.

**Response `data`** — mismo shape que un item de la lista.

---

## 4. Eliminar prenda

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | `/prenda/{id}` | `/api/prendas/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request** — sin body, `id` en la ruta.

**Flujo interno actual** — el handler ejecuta en orden:
1. Verifica que no tenga descuentos, tallas ni reseñas asociadas (devuelve `409 Conflict` si las tiene)
2. Llama `DeleteByPrefixAsync("prendas/{id}/")` → lista y borra en batch todos los archivos del bucket R2
3. Elimina los registros `PrendaImagen` de BD
4. Elimina la prenda de BD

> En el sistema antiguo el frontend llamaba `eliminarCarpeta` por separado antes de hacer el DELETE. Ahora todo ocurre en una sola llamada al backend.

**Bloqueos que ya NO aplican:**
- ~~"La prenda tiene imágenes asociadas"~~ → se eliminan automáticamente

**Bloqueos que siguen activos:**
- `409` si tiene descuentos asociados
- `409` si tiene tallas asociadas
- `409` si tiene reseñas asociadas

**Response `data`**
```json
true
```

---

## 5. Detalle completo de prenda (NUEVO)

> No existía en la versión antigua.

| | Versión actual |
|---|---|
| Método | `GET` |
| Ruta | `/api/prendas/{id}/detalle` |
| Auth | Bearer |

**Response `data`**
```json
{
  "id": 1,
  "nombre": "Polo Oversize",
  "descripcion": "...",
  "precio": 89.90,
  "activo": true,
  "createdAt": "2025-01-01T00:00:00",
  "updatedAt": null,
  "imagenPrincipal": "https://pub-xxx.r2.dev/prendas/1/uuid.jpg",
  "imagenHover": "https://pub-xxx.r2.dev/prendas/1/uuid2.jpg",
  "imagenVideo": null,
  "imagenExtra1": null,
  "imagenExtra2": null,
  "marca":      { "id": 2, "nomMarca": "Nike" },
  "categoria":  { "id": 3, "nomCategoria": "Polos" },
  "proveedor":  { "id": 1, "nomProveedor": "Proveedor SA" },
  "tallas": [
    { "prendaTallaId": 10, "tallaId": 3, "nomTalla": "M", "stock": 15 }
  ]
}
```

---

## 6. Subir imagen a R2 (reemplaza `uploadArchivo`)

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/archivos/upload` | `/api/prendas/{id}/imagenes` |
| Content-Type | `multipart/form-data` | `multipart/form-data` |
| Auth | Bearer | Bearer (ADMIN) |

**Form fields actuales**
```
archivo   File      requerido — el archivo de imagen
tipo      string    requerido — PRINCIPAL | SECUNDARIA | DETALLE
orden     integer   opcional
```

**Response `data`**
```json
{
  "id": 5,
  "prendaId": 1,
  "tipo": "PRINCIPAL",
  "url": "https://pub-xxx.r2.dev/prendas/1/550e8400-e29b-41d4-a716.jpg",
  "orden": null,
  "createdAt": "2025-06-29T00:00:00"
}
```

---

## 7. Crear imagen con URL directa (sin subida)

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `POST` | `POST` |
| Ruta | `/imagen` | `/api/prendaImagenes` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{
  "prendaId": 1,
  "tipo": "PRINCIPAL",
  "url": "https://pub-xxx.r2.dev/prendas/1/imagen.jpg",
  "orden": 1
}
```

**Response `data`**
```json
{
  "id": 5,
  "prendaId": 1,
  "tipo": "PRINCIPAL",
  "url": "https://pub-xxx.r2.dev/prendas/1/imagen.jpg",
  "orden": 1,
  "createdAt": "2025-06-29T00:00:00",
  "updatedAt": null
}
```

---

## 8. Actualizar imagen

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `PUT` | `PUT` |
| Ruta | `/imagen/{id}` | `/api/prendaImagenes/{id}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request body**
```json
{
  "tipo": "SECUNDARIA",
  "url": "https://pub-xxx.r2.dev/prendas/1/nueva.jpg",
  "orden": 2
}
```

**Response `data`** — mismo shape que crear imagen.

---

## 9. Eliminar imagen (borra de R2 + BD)

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `DELETE` | `DELETE` |
| Ruta | *(no existía)* | `/api/prendaImagenes/{id}` |
| Alternativa | *(no existía)* | `DELETE /api/prendas/{id}/imagenes/{imagenId}` |
| Auth | Bearer | Bearer (ADMIN) |

**Request** — sin body, `id` en la ruta.

**Response `data`**
```json
true
```

> Ambos endpoints de delete bajan el archivo de Cloudflare R2 antes de eliminar el registro en BD.

---

## 10. Listar géneros

| | Versión antigua | Versión actual |
|---|---|---|
| Método | `GET` | `GET` |
| Ruta | `/generos` | `/api/generos` |
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
    { "id": 1, "nomGenero": "Hombre" },
    { "id": 2, "nomGenero": "Mujer" }
  ],
  "totalCount": 3
}
```
