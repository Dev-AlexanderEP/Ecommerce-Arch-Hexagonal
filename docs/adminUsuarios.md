# Módulo de Usuarios

Gestión de cuentas de usuario: CRUD completo con filtros por nombre, email, rol y estado.

Base URL: `/api/Usuarios`

> Todos los endpoints requieren `Authorization: Bearer <token>` con rol **ADMIN**.

---

## Modelo de respuesta

### `UsuarioResponseDto`

```json
{
  "id": 1,
  "nombreUsuario": "alexander",
  "email": "alexander@gmail.com",
  "rol": "CLIENTE",
  "activo": true,
  "createdAt": "2026-06-01T10:00:00Z",
  "updatedAt": null
}
```

| Campo          | Tipo     | Descripción                        |
|----------------|----------|------------------------------------|
| `id`           | long     | Identificador único                |
| `nombreUsuario`| string   | Nombre de usuario                  |
| `email`        | string   | Correo electrónico                 |
| `rol`          | string   | `CLIENTE` \| `ADMIN`              |
| `activo`       | bool     | Si la cuenta está habilitada       |
| `createdAt`    | datetime | Fecha de creación (UTC)            |
| `updatedAt`    | datetime | Última actualización (UTC) o null  |

---

## Endpoints

### `GET /api/Usuarios`

Lista paginada de usuarios con filtros opcionales. Todos los filtros son combinables.

**Query params:**

| Parámetro  | Tipo   | Descripción                                          |
|------------|--------|------------------------------------------------------|
| `nombre`   | string | Búsqueda parcial en `nombreUsuario` (case-insensitive) |
| `email`    | string | Búsqueda parcial en `email` (case-insensitive)       |
| `rol`      | string | Filtro exacto: `CLIENTE` o `ADMIN`                   |
| `activo`   | bool   | Filtro por estado: `true` o `false`                  |
| `page`     | int    | Número de página (default: `1`)                      |
| `pageSize` | int    | Tamaño de página (default: `10`)                     |

**Ejemplos:**

```
GET /api/Usuarios
GET /api/Usuarios?nombre=alex
GET /api/Usuarios?rol=CLIENTE&activo=true
GET /api/Usuarios?email=gmail&page=1&pageSize=20
GET /api/Usuarios?nombre=ale&rol=ADMIN&activo=true
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "nombreUsuario": "alexander",
        "email": "alexander@gmail.com",
        "rol": "CLIENTE",
        "activo": true,
        "createdAt": "2026-06-01T10:00:00Z",
        "updatedAt": null
      }
    ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 10
  }
}
```

> Resultado ordenado alfabéticamente por `nombreUsuario`. Una lista vacía devuelve 200 con `items: []`.

---

### `GET /api/Usuarios/{id}`

Retorna un usuario por ID.

**Response 200:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "nombreUsuario": "alexander",
    "email": "alexander@gmail.com",
    "rol": "CLIENTE",
    "activo": true,
    "createdAt": "2026-06-01T10:00:00Z",
    "updatedAt": null
  }
}
```

**Response 404:**
```json
{
  "success": false,
  "message": "Usuario no encontrado para id 99."
}
```

---

### `POST /api/Usuarios`

Crea un nuevo usuario. Al crearse, se envía un correo de bienvenida automáticamente (si el SMTP está configurado).

**Request body:**
```json
{
  "nombreUsuario": "alexander",
  "email": "alexander@gmail.com",
  "contrasenia": "Passw0rd!",
  "rol": "CLIENTE",
  "activo": true
}
```

| Campo          | Tipo   | Requerido | Descripción                             |
|----------------|--------|-----------|-----------------------------------------|
| `nombreUsuario`| string | Sí        | Debe ser único                          |
| `email`        | string | Sí        | Debe ser único y formato válido         |
| `contrasenia`  | string | Sí        | Se almacena hasheada                    |
| `rol`          | string | No        | `CLIENTE` (default) \| `ADMIN`         |
| `activo`       | bool   | No        | `true` por defecto                      |

**Response 200:**
```json
{
  "success": true,
  "data": { /* UsuarioResponseDto */ }
}
```

**Response 409 — email o nombre duplicado:**
```json
{
  "success": false,
  "message": "Ya existe un usuario con el email alexander@gmail.com."
}
```

**Response 400 — validación:**
```json
{
  "success": false,
  "message": "El campo Email no tiene un formato válido."
}
```

---

### `PUT /api/Usuarios/{id}`

Actualiza los datos de un usuario. El campo `nuevaContrasenia` es opcional: si se omite, la contraseña no cambia.

**Request body:**
```json
{
  "nombreUsuario": "alexander_ep",
  "email": "nuevo@gmail.com",
  "rol": "ADMIN",
  "activo": true,
  "nuevaContrasenia": "NuevoPass123!"
}
```

| Campo             | Tipo   | Requerido | Descripción                                       |
|-------------------|--------|-----------|---------------------------------------------------|
| `nombreUsuario`   | string | Sí        | Nuevo nombre (debe ser único entre otros usuarios)|
| `email`           | string | Sí        | Nuevo email (debe ser único entre otros usuarios) |
| `rol`             | string | No        | `CLIENTE` \| `ADMIN`. Si se omite, conserva el actual |
| `activo`          | bool   | Sí        | Estado de la cuenta                               |
| `nuevaContrasenia`| string | No        | Si se omite, la contraseña no cambia              |

**Response 200:**
```json
{
  "success": true,
  "data": { /* UsuarioResponseDto actualizado */ }
}
```

**Response 404:**
```json
{
  "success": false,
  "message": "Usuario no encontrado para id 99."
}
```

**Response 409 — conflicto con otro usuario:**
```json
{
  "success": false,
  "message": "El email nuevo@gmail.com ya esta en uso por otro usuario."
}
```

---

### `DELETE /api/Usuarios/{id}`

Elimina un usuario por ID.

**Response 200:**
```json
{
  "success": true,
  "data": null
}
```

**Response 404:**
```json
{
  "success": false,
  "message": "Usuario no encontrado para id 99."
}
```

---

## Resumen de endpoints

| Método   | Ruta                  | Descripción                        |
|----------|-----------------------|------------------------------------|
| `GET`    | `/api/Usuarios`       | Lista paginada con filtros         |
| `GET`    | `/api/Usuarios/{id}`  | Por ID                             |
| `POST`   | `/api/Usuarios`       | Crear usuario                      |
| `PUT`    | `/api/Usuarios/{id}`  | Actualizar usuario                 |
| `DELETE` | `/api/Usuarios/{id}`  | Eliminar usuario                   |

---

## Filtros del GET — referencia rápida

| Caso de uso                        | Query                                        |
|------------------------------------|----------------------------------------------|
| Todos los usuarios                 | `GET /api/Usuarios`                          |
| Buscar por nombre                  | `?nombre=alex`                               |
| Buscar por email                   | `?email=gmail`                               |
| Solo clientes activos              | `?rol=CLIENTE&activo=true`                   |
| Solo admins                        | `?rol=ADMIN`                                 |
| Usuarios inactivos                 | `?activo=false`                              |
| Combinado con paginación           | `?nombre=ale&rol=CLIENTE&page=2&pageSize=5`  |
