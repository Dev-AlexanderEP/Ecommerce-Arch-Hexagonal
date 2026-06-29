# Módulo Auth

Gestiona el registro, inicio de sesión (credenciales y Google) y cambio de contraseña. Todos los endpoints devuelven el formato descrito en [api-responses.md](./api-responses.md).

**Base URL:** `/api/auth`

---

## Endpoints

| Método | Ruta                        | Autenticación | Descripción                         |
|--------|-----------------------------|---------------|-------------------------------------|
| POST   | `/api/auth/register`        | Público       | Registra un nuevo usuario CLIENTE.  |
| POST   | `/api/auth/login`           | Público       | Login con email y contraseña.       |
| POST   | `/api/auth/google`          | Público       | Login / registro con Google.        |
| POST   | `/api/auth/change-password` | Bearer JWT    | Cambia la contraseña del usuario autenticado. |

---

## POST /api/auth/register

Crea una cuenta nueva con rol `CLIENTE` y devuelve un JWT listo para usar.

### Request body

```json
{
  "nombreUsuario": "jdoe",
  "email": "jdoe@ejemplo.com",
  "contrasenia": "secreto123"
}
```

| Campo          | Tipo     | Requerido | Validación                            |
|----------------|----------|-----------|---------------------------------------|
| `nombreUsuario`| `string` | Sí        | No vacío. Máximo 255 caracteres.      |
| `email`        | `string` | Sí        | Formato email válido. Máximo 255 caracteres. |
| `contrasenia`  | `string` | Sí        | Mínimo 8 caracteres.                  |

### Response `201 Created`

```json
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
    "expiraEn": "2025-07-05T12:00:00Z",
    "id": 42,
    "nombreUsuario": "jdoe",
    "email": "jdoe@ejemplo.com",
    "rol": "CLIENTE"
  },
  "error": null,
  "status": "Created"
}
```

Ver [AuthResponseDto](#authresponsedto).

### Errores posibles

| HTTP | `error`      | Cuándo                                                |
|------|--------------|-------------------------------------------------------|
| 400  | —            | Campos inválidos (FluentValidation) → formato `errors`|
| 409  | `Conflict`   | El email o el `nombreUsuario` ya está registrado.     |

---

## POST /api/auth/login

Autentica con email y contraseña. Funciona solo para cuentas con credencial local (no cuentas creadas con Google).

### Request body

```json
{
  "email": "jdoe@ejemplo.com",
  "contrasenia": "secreto123"
}
```

| Campo         | Tipo     | Requerido | Validación               |
|---------------|----------|-----------|--------------------------|
| `email`       | `string` | Sí        | Formato email válido.    |
| `contrasenia` | `string` | Sí        | No vacío.                |

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
    "expiraEn": "2025-07-05T12:00:00Z",
    "id": 42,
    "nombreUsuario": "jdoe",
    "email": "jdoe@ejemplo.com",
    "rol": "CLIENTE"
  },
  "error": null,
  "status": "Ok"
}
```

### Errores posibles

| HTTP | `error`        | Cuándo                                                    |
|------|----------------|-----------------------------------------------------------|
| 400  | —              | Campos inválidos (FluentValidation) → formato `errors`    |
| 401  | `Unauthorized` | Email no registrado o contraseña incorrecta.              |
| 403  | `Forbidden`    | La cuenta existe pero está inactiva.                      |

> El servidor responde con `"Credenciales inválidas."` tanto para email inexistente como para contraseña incorrecta, sin indicar cuál falló.

---

## POST /api/auth/google

Autentica o registra un usuario usando un `idToken` de Google. Si el email no existe, crea la cuenta automáticamente con rol `CLIENTE` y sin contraseña local.

### Request body

El body es un string JSON (no un objeto):

```json
"eyJhbGciOiJSUzI1NiIsImtpZCI6Ij..."
```

| Campo     | Tipo     | Descripción                                        |
|-----------|----------|----------------------------------------------------|
| *(body)*  | `string` | `idToken` obtenido del flujo de Google Sign-In.    |

### Response `200 OK`

Mismo formato que `/login`. Ver [AuthResponseDto](#authresponsedto).

### Errores posibles

| HTTP | `error`        | Cuándo                                          |
|------|----------------|-------------------------------------------------|
| 401  | `Unauthorized` | El `idToken` de Google no es válido o expiró.   |
| 403  | `Forbidden`    | La cuenta existe pero está inactiva.            |

> Si el email de Google ya existe como cuenta local (con contraseña), el login con Google también funciona — el sistema busca por email, no por proveedor.

---

## POST /api/auth/change-password

Cambia la contraseña del usuario autenticado. Requiere Bearer JWT. Solo funciona en cuentas con contraseña local — las cuentas Google no tienen contraseña local.

### Headers

```
Authorization: Bearer <token>
```

### Request body

```json
{
  "contraseniaActual": "secreto123",
  "contraseniaNueva": "nuevoSecreto456"
}
```

| Campo              | Tipo     | Requerido | Validación                                        |
|--------------------|----------|-----------|---------------------------------------------------|
| `contraseniaActual`| `string` | Sí        | No vacío.                                         |
| `contraseniaNueva` | `string` | Sí        | Mínimo 8 caracteres. Debe ser distinta a la actual.|

> El campo `usuarioId` es asignado internamente por el servidor desde el JWT — no se envía en el body.

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": true,
  "error": null,
  "status": "Ok"
}
```

### Errores posibles

| HTTP | `error`      | Cuándo                                                    |
|------|--------------|-----------------------------------------------------------|
| 400  | —            | Campos inválidos (FluentValidation) → formato `errors`    |
| 400  | `Validation` | La contraseña actual no coincide con la registrada.       |
| 400  | `Validation` | La cuenta es de Google y no tiene contraseña local.       |
| 401  | —            | Token JWT ausente o expirado (ASP.NET Core).              |
| 404  | `NotFound`   | El usuario del token no existe en BD.                     |

---

## AuthResponseDto

Estructura del campo `data` devuelto por `/register`, `/login` y `/google`.

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiraEn": "2025-07-05T12:00:00Z",
  "id": 42,
  "nombreUsuario": "jdoe",
  "email": "jdoe@ejemplo.com",
  "rol": "CLIENTE"
}
```

| Campo          | Tipo       | Descripción                                              |
|----------------|------------|----------------------------------------------------------|
| `token`        | `string`   | JWT Bearer para incluir en `Authorization: Bearer <token>`. |
| `expiraEn`     | `datetime` | Fecha/hora UTC de expiración del token.                  |
| `id`           | `long`     | ID interno del usuario.                                  |
| `nombreUsuario`| `string`   | Nombre de usuario único.                                 |
| `email`        | `string`   | Email del usuario.                                       |
| `rol`          | `string?`  | Rol asignado: `"CLIENTE"` o `"ADMIN"`.                   |
