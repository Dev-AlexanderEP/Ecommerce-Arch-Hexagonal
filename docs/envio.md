# Módulo Envío

Gestiona los datos de entrega del comprador (`DatosEnvio`) y los registros de envío de cada pedido (`Envio`). Todos los endpoints devuelven el formato descrito en [api-responses.md](./api-responses.md).

---

## Endpoints — DatosEnvio

**Base URL:** `/api/datos-envio`

| Método | Ruta                                  | Autenticación  | Descripción                                           |
|--------|---------------------------------------|----------------|-------------------------------------------------------|
| GET    | `/api/datos-envio/mis-direcciones`    | CLIENTE        | Devuelve las direcciones del usuario autenticado.     |
| GET    | `/api/datos-envio`                    | ADMIN          | Lista paginada de todos los datos de envío.           |
| GET    | `/api/datos-envio/{id}`               | CLIENTE        | Datos de envío por ID. Solo propios.                  |
| POST   | `/api/datos-envio`                    | CLIENTE        | Registra una nueva dirección de entrega.              |
| PUT    | `/api/datos-envio/{id}`               | CLIENTE        | Actualiza una dirección propia.                       |
| DELETE | `/api/datos-envio/{id}`               | CLIENTE        | Elimina una dirección propia.                         |

---

## Endpoints — Envio

**Base URL:** `/api/envio`

| Método | Ruta                                          | Autenticación | Descripción                                                  |
|--------|-----------------------------------------------|---------------|--------------------------------------------------------------|
| GET    | `/api/envio`                                  | ADMIN         | Lista paginada de todos los envíos.                          |
| GET    | `/api/envio/{id}`                             | ADMIN         | Envío por ID.                                                |
| POST   | `/api/envio`                                  | ADMIN         | Registra un envío para una venta.                            |
| PUT    | `/api/envio/{id}`                             | ADMIN         | Actualiza un envío existente.                                |
| DELETE | `/api/envio/{id}`                             | ADMIN         | Elimina un envío.                                            |
| GET    | `/api/envio/tracking/{trackingNumber}`        | CLIENTE       | Consulta el estado de un envío por número de seguimiento.    |
| GET    | `/api/envio/usuario/{userId}/entregados`      | ADMIN         | Envíos entregados de un usuario específico.                  |
| GET    | `/api/envio/usuario/{userId}/no-entregados`   | ADMIN         | Envíos pendientes/en camino de un usuario específico.        |

---

## GET /api/datos-envio/mis-direcciones

Devuelve todas las direcciones registradas por el usuario autenticado. La dirección marcada como principal aparece primero.

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
      "usuarioId": 42,
      "nombres": "Juan",
      "apellidos": "Pérez",
      "dni": "12345678",
      "departamento": "Lima",
      "provincia": "Lima",
      "distrito": "Miraflores",
      "calle": "Av. Larco",
      "detalle": "Piso 3, depto 301",
      "telefono": "987654321",
      "email": "juan@ejemplo.com",
      "esPrincipal": true,
      "createdAt": "2025-06-01T12:00:00Z",
      "updatedAt": null
    }
  ],
  "error": null,
  "status": "Ok"
}
```

> `data` puede ser `[]` si el usuario no ha registrado ninguna dirección — no es un error.

---

## GET /api/datos-envio/{id}

Devuelve un registro propio. Si el `id` pertenece a otro usuario devuelve `403 Forbidden`.

### Errores posibles

| HTTP | `error`     | Cuándo                                         |
|------|-------------|------------------------------------------------|
| 403  | `Forbidden` | El registro pertenece a otro usuario.          |
| 404  | `NotFound`  | No existe un registro con ese ID.              |

---

## POST /api/datos-envio

Registra una nueva dirección de entrega. El `usuarioId` se obtiene del JWT — no se envía en el body.

### Request body

```json
{
  "nombres": "Juan",
  "apellidos": "Pérez",
  "dni": "12345678",
  "departamento": "Lima",
  "provincia": "Lima",
  "distrito": "Miraflores",
  "calle": "Av. Larco",
  "detalle": "Piso 3, depto 301",
  "telefono": "987654321",
  "email": "juan@ejemplo.com"
}
```

| Campo         | Tipo     | Requerido | Descripción                                      |
|---------------|----------|-----------|--------------------------------------------------|
| `nombres`     | `string` | Sí        |                                                  |
| `apellidos`   | `string` | Sí        |                                                  |
| `dni`         | `string` | Sí        |                                                  |
| `departamento`| `string` | Sí        |                                                  |
| `provincia`   | `string` | Sí        |                                                  |
| `distrito`    | `string` | Sí        |                                                  |
| `calle`       | `string` | No        | Puede omitirse.                                  |
| `detalle`     | `string` | Sí        | Referencia / complemento de la dirección.        |
| `telefono`    | `string` | Sí        |                                                  |
| `email`       | `string` | Sí        | Email de contacto para el envío.                 |

> El primer registro creado queda marcado como `esPrincipal = true` automáticamente.

### Response `201 Created`

Devuelve el `DatosEnvioResponseDto` creado (ver [DTOs](#datosenvioresponsedto)).

### Errores posibles

| HTTP | `error`    | Cuándo                                              |
|------|------------|-----------------------------------------------------|
| 400  | —          | Campos inválidos (FluentValidation).                |
| 409  | `Conflict` | El usuario ya tiene una dirección registrada.       |

---

## PUT /api/datos-envio/{id}

Actualiza la dirección propia. El body tiene los mismos campos que el POST (todos requeridos salvo `calle`).

### Errores posibles

| HTTP | `error`     | Cuándo                                         |
|------|-------------|------------------------------------------------|
| 400  | —           | Campos inválidos.                              |
| 403  | `Forbidden` | El registro pertenece a otro usuario.          |
| 404  | `NotFound`  | No existe un registro con ese ID.              |

---

## DELETE /api/datos-envio/{id}

Elimina la dirección propia. Devuelve `204 No Content`.

### Errores posibles

| HTTP | `error`     | Cuándo                                         |
|------|-------------|------------------------------------------------|
| 403  | `Forbidden` | El registro pertenece a otro usuario.          |
| 404  | `NotFound`  | No existe un registro con ese ID.              |

---

## GET /api/envio/tracking/{trackingNumber}

Permite al CLIENTE consultar el estado de su pedido usando el número de seguimiento entregado en el checkout.

### Parámetros

| Parámetro        | Tipo     | Descripción                         |
|------------------|----------|-------------------------------------|
| `trackingNumber` | `string` | Número de seguimiento del envío.    |

### Response `200 OK`

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 5,
    "ventaId": 12,
    "datosEnvioId": 1,
    "costoEnvio": 0.00,
    "fechaEnvio": "2025-06-01",
    "fechaEntrega": "2025-06-30",
    "estado": "EN_CAMINO",
    "metodoEnvio": "Delivery",
    "trackingNumber": "A3B7C2D9E1",
    "createdAt": "2025-06-01T10:00:00Z",
    "updatedAt": null
  },
  "error": null,
  "status": "Ok"
}
```

### Errores posibles

| HTTP | `error`    | Cuándo                                              |
|------|------------|-----------------------------------------------------|
| 404  | `NotFound` | No existe un envío con ese tracking number.         |

---

## POST /api/envio  *(solo ADMIN)*

Registra el envío de una venta. Una venta solo puede tener un envío — el segundo intento devuelve `409`.

### Request body

```json
{
  "ventaId": 12,
  "datosEnvioId": 1,
  "costoEnvio": 0.00,
  "fechaEnvio": "2025-06-01",
  "fechaEntrega": "2025-06-30",
  "estado": "PREPARANDO",
  "metodoEnvio": "Delivery",
  "trackingNumber": "A3B7C2D9E1"
}
```

| Campo           | Tipo      | Requerido | Descripción                                                          |
|-----------------|-----------|-----------|----------------------------------------------------------------------|
| `ventaId`       | `long`    | Sí        | ID de la venta asociada.                                             |
| `datosEnvioId`  | `long`    | Sí        | ID de los datos de entrega del comprador.                            |
| `costoEnvio`    | `decimal` | Sí        | Costo del envío (puede ser `0`).                                     |
| `fechaEnvio`    | `date`    | Sí        | Fecha de despacho. Formato `YYYY-MM-DD`.                             |
| `fechaEntrega`  | `date`    | No        | Fecha estimada de entrega.                                           |
| `estado`        | `string`  | Sí        | Uno de: `PREPARANDO`, `EN_CAMINO`, `ENTREGADO`, `DEVUELTO`.         |
| `metodoEnvio`   | `string`  | Sí        | Ej: `"Delivery"`, `"Recojo en tienda"`.                              |
| `trackingNumber`| `string`  | No        | Código de seguimiento. Si se omite el cliente no podrá usar tracking.|

### Response `201 Created`

Devuelve el `EnvioResponseDto` creado (ver [DTOs](#envioresponsedto)).

### Errores posibles

| HTTP | `error`      | Cuándo                                                    |
|------|--------------|-----------------------------------------------------------|
| 400  | `Validation` | `ventaId` o `datosEnvioId` no existen.                    |
| 400  | `Validation` | `estado` no es un valor válido del enum.                  |
| 409  | `Conflict`   | La venta ya tiene un envío registrado.                    |

---

## PUT /api/envio/{id}  *(solo ADMIN)*

Actualiza todos los campos de un envío. El body tiene los mismos campos que el POST.

---

## DTOs

### DatosEnvioResponseDto

Devuelto por todos los endpoints de `DatosEnvio`.

```json
{
  "id": 1,
  "usuarioId": 42,
  "nombres": "Juan",
  "apellidos": "Pérez",
  "dni": "12345678",
  "departamento": "Lima",
  "provincia": "Lima",
  "distrito": "Miraflores",
  "calle": "Av. Larco",
  "detalle": "Piso 3, depto 301",
  "telefono": "987654321",
  "email": "juan@ejemplo.com",
  "esPrincipal": true,
  "createdAt": "2025-06-01T12:00:00Z",
  "updatedAt": null
}
```

| Campo         | Tipo        | Descripción                                                  |
|---------------|-------------|--------------------------------------------------------------|
| `id`          | `long`      | ID interno.                                                  |
| `usuarioId`   | `long`      | ID del usuario propietario.                                  |
| `nombres`     | `string`    |                                                              |
| `apellidos`   | `string`    |                                                              |
| `dni`         | `string`    |                                                              |
| `departamento`| `string`    |                                                              |
| `provincia`   | `string`    |                                                              |
| `distrito`    | `string`    |                                                              |
| `calle`       | `string?`   | Puede ser `null`.                                            |
| `detalle`     | `string`    | Referencia / piso / complemento.                             |
| `telefono`    | `string`    |                                                              |
| `email`       | `string`    | Email de contacto para el envío.                             |
| `esPrincipal` | `boolean`   | `true` si es la dirección principal del usuario.             |
| `createdAt`   | `datetime`  |                                                              |
| `updatedAt`   | `datetime?` |                                                              |

---

### EnvioResponseDto

Devuelto por todos los endpoints de `Envio`.

```json
{
  "id": 5,
  "ventaId": 12,
  "datosEnvioId": 1,
  "costoEnvio": 0.00,
  "fechaEnvio": "2025-06-01",
  "fechaEntrega": "2025-06-30",
  "estado": "EN_CAMINO",
  "metodoEnvio": "Delivery",
  "trackingNumber": "A3B7C2D9E1",
  "createdAt": "2025-06-01T10:00:00Z",
  "updatedAt": null
}
```

| Campo           | Tipo        | Descripción                                                          |
|-----------------|-------------|----------------------------------------------------------------------|
| `id`            | `long`      | ID interno.                                                          |
| `ventaId`       | `long`      | ID de la venta asociada.                                             |
| `datosEnvioId`  | `long`      | ID de los datos de entrega.                                          |
| `costoEnvio`    | `decimal`   |                                                                      |
| `fechaEnvio`    | `date`      | Formato `YYYY-MM-DD`.                                                |
| `fechaEntrega`  | `date?`     | Puede ser `null`.                                                    |
| `estado`        | `string`    | `PREPARANDO` · `EN_CAMINO` · `ENTREGADO` · `DEVUELTO`               |
| `metodoEnvio`   | `string`    |                                                                      |
| `trackingNumber`| `string?`   | Puede ser `null` si el ADMIN no lo asignó al crear.                  |
| `createdAt`     | `datetime`  |                                                                      |
| `updatedAt`     | `datetime?` |                                                                      |

---

## Migración desde el frontend antiguo

| Endpoint JS (Spring)                              | .NET nuevo                                    | Cambios clave                                              |
|---------------------------------------------------|-----------------------------------------------|------------------------------------------------------------|
| `GET /direcciones/usuario/${usuarioId}`           | `GET /api/datos-envio/mis-direcciones`        | El `usuarioId` ya no va en la URL — lo extrae el JWT.      |
| `POST /dato-personal`                             | `POST /api/datos-envio`                       | Renombrado. Mismo body, `usuarioId` del JWT.               |
| `POST /direccion`                                 | `POST /api/datos-envio`                       | Mismo endpoint que `dato-personal` — el modelo es único.   |
| `POST /envio`                                     | `POST /api/envio`                             | Ahora solo ADMIN. El CLIENTE no crea envíos directamente.  |
| `GET /envio/tracking/${codigo}`                   | `GET /api/envio/tracking/{trackingNumber}`    | Solo CLIENTE (antes era ADMIN).                            |
| `GET /registrar?id=${envioId}`                    | *(email automático)*                          | Ya no es un endpoint. El email se envía al crear la venta. |
