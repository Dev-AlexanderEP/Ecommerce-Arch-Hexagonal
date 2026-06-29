# Contrato de Respuesta de la API

Todos los endpoints de MixAndMatch devuelven una de dos estructuras de envelope, más un formato especial para errores de validación de FluentValidation.

---

## ApiResponse\<T\>

Usado por endpoints que devuelven un único recurso (GET por ID, POST, PUT, PATCH, DELETE).

```json
{
  "success": true,
  "message": "Mensaje opcional",
  "data": { ... },
  "error": null,
  "status": "Ok"
}
```

| Campo     | Tipo      | Descripción                                                  |
|-----------|-----------|--------------------------------------------------------------|
| `success` | `boolean` | `true` si la operación fue exitosa, `false` si falló.        |
| `message` | `string?` | Mensaje descriptivo. Presente en errores, opcional en éxitos.|
| `data`    | `T?`      | Payload. `null` cuando `success` es `false`.                 |
| `error`   | `string?` | Tipo de error semántico. `null` cuando `success` es `true`.  |
| `status`  | `string`  | Tipo de éxito. Solo relevante cuando `success` es `true`.    |

### Valores de `status` (solo en éxito)

| Valor       | HTTP | Cuándo se usa                                      |
|-------------|------|----------------------------------------------------|
| `Ok`        | 200  | GET, UPDATE, operaciones que devuelven datos.       |
| `Created`   | 201  | POST que crea un recurso nuevo.                    |
| `NoContent` | 204  | Operaciones exitosas sin cuerpo de respuesta. El envelope **no** se incluye. |

---

## ApiPaginationResponse\<T\>

Usado por endpoints que devuelven listas paginadas (GET con filtros y paginación).

```json
{
  "success": true,
  "message": null,
  "data": [ { ... }, { ... } ],
  "metadata": {
    "totalCount": 42,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNext": true,
    "hasPrev": false
  },
  "error": null
}
```

| Campo      | Tipo             | Descripción                            |
|------------|------------------|----------------------------------------|
| `success`  | `boolean`        | `true` si la operación fue exitosa.    |
| `message`  | `string?`        | Mensaje descriptivo opcional.          |
| `data`     | `array<T>`       | Lista de resultados. Puede ser `[]` si no hay resultados — esto **no** es un error. |
| `metadata` | `object`         | Información de paginación (ver abajo). |
| `error`    | `string?`        | Tipo de error semántico.               |

#### Objeto `metadata`

| Campo        | Tipo      | Descripción                                         |
|--------------|-----------|-----------------------------------------------------|
| `totalCount` | `integer` | Total de registros que coinciden con los filtros.   |
| `page`       | `integer` | Página actual solicitada.                           |
| `pageSize`   | `integer` | Tamaño de página solicitado.                        |
| `totalPages` | `integer` | Total de páginas: `ceil(totalCount / pageSize)`.    |
| `hasNext`    | `boolean` | `true` si hay una página siguiente.                 |
| `hasPrev`    | `boolean` | `true` si hay una página anterior.                  |

---

## Códigos HTTP

### Respuestas exitosas

| Código | Cuándo                                          |
|--------|-------------------------------------------------|
| `200`  | Operación exitosa con datos.                    |
| `201`  | Recurso creado exitosamente.                    |
| `204`  | Operación exitosa sin cuerpo (ej. DELETE).      |

### Respuestas de error — envelope `ApiResponse<T>`

Cuando la capa de aplicación detecta un error de negocio (recurso no encontrado, conflicto, etc.), el handler devuelve `ApiResponse.Fail(...)` con un `ErrorType`. El campo `error` del envelope indica el tipo:

| `error`        | HTTP | Descripción                                                    |
|----------------|------|----------------------------------------------------------------|
| `NotFound`     | 404  | El recurso solicitado no existe.                               |
| `Validation`   | 400  | Regla de negocio incumplida sobre el input (ej. prenda inexistente). |
| `Unauthorized` | 401  | Credenciales ausentes o incorrectas.                           |
| `Forbidden`    | 403  | Autenticado pero sin permiso, o cuenta inactiva.               |
| `Conflict`     | 409  | Choca con estado existente (ej. email duplicado).              |

Ejemplo de error de negocio:
```json
{
  "success": false,
  "message": "Ya existe un usuario con el email usuario@ejemplo.com.",
  "data": null,
  "error": "Conflict",
  "status": "Ok"
}
```

---

## Errores de validación de FluentValidation

Cuando los campos del request no pasan las reglas de FluentValidation (antes de llegar al handler), se devuelve un `ProblemDetails` estándar con los errores agrupados por campo. **Este formato es distinto al envelope `ApiResponse<T>`.**

```json
{
  "status": 400,
  "title": "Error de validación",
  "errors": {
    "Email": ["El email no tiene un formato válido."],
    "Contrasenia": ["La contraseña debe tener al menos 8 caracteres."]
  }
}
```

| Campo    | Tipo              | Descripción                                         |
|----------|-------------------|-----------------------------------------------------|
| `status` | `integer`         | Siempre `400`.                                      |
| `title`  | `string`          | Siempre `"Error de validación"`.                    |
| `errors` | `object`          | Clave: nombre del campo. Valor: array de mensajes.  |

---

## Errores no controlados

Excepciones no capturadas por la capa de aplicación son interceptadas por el `GlobalExceptionHandler` y devueltas como `ProblemDetails`:

```json
{
  "status": 500,
  "title": "Error interno del servidor",
  "detail": "Mensaje de la excepción."
}
```

| Excepción                  | HTTP | `title`                      |
|----------------------------|------|------------------------------|
| `KeyNotFoundException`     | 404  | Recurso no encontrado        |
| `ArgumentException`        | 400  | Solicitud inválida           |
| `UnauthorizedAccessException` | 401 | No autorizado             |
| `InvalidOperationException`| 422  | Operación inválida           |
| Cualquier otra             | 500  | Error interno del servidor   |
