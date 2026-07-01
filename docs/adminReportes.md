# Módulo de Reportes — Admin

Endpoints de descarga de archivos Excel (`.xlsx`) generados con ClosedXML para el panel de administración. Cada reporte incluye múltiples hojas con formato, colores y resúmenes agrupados.

Base URL: `/api/Reportes`

> Todos los endpoints requieren `Authorization: Bearer <token>` con rol **ADMIN**.  
> La respuesta es un archivo binario `.xlsx`, no JSON.

---

## Tabla de contenidos

1. [Reporte de Stock](#1-reporte-de-stock)
2. [Reporte de Ventas](#2-reporte-de-ventas)
3. [Resumen de endpoints](#resumen-de-endpoints)
4. [Contenido de los archivos](#contenido-de-los-archivos)

---

## 1. Reporte de Stock

```
GET /api/Reportes/stock
```

Genera un Excel con el inventario actual de todas las prendas activas, con filtros opcionales combinables.

### Query params

| Parámetro   | Tipo   | Requerido | Descripción                                 |
|-------------|--------|-----------|---------------------------------------------|
| `nombre`    | string | No        | Filtra por nombre de prenda (parcial, sin distinción de mayúsculas) |
| `genero`    | string | No        | Filtra por nombre de género (ej. `Mujer`)   |
| `categoria` | string | No        | Filtra por nombre de categoría (ej. `Tops`) |

> Los tres filtros son opcionales e independientes. Si no se envía ninguno, el reporte incluye todas las prendas activas.

### Ejemplos de uso

```
GET /api/Reportes/stock
GET /api/Reportes/stock?nombre=polo
GET /api/Reportes/stock?genero=Mujer
GET /api/Reportes/stock?categoria=Tops
GET /api/Reportes/stock?nombre=polo&genero=Mujer&categoria=Tops
```

### Response 200

```
Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
Content-Disposition: attachment; filename="stock_20260701_1430.xlsx"
```

El nombre del archivo incluye fecha y hora de generación (`yyyyMMdd_HHmm`).

### Errores

| Código | Descripción                              |
|--------|------------------------------------------|
| `401`  | Token no enviado o expirado              |
| `403`  | El usuario autenticado no tiene rol ADMIN|

---

## 2. Reporte de Ventas

```
GET /api/Reportes/ventas
```

Genera un Excel con las ventas realizadas (`PAGADO`, `ENVIADO` o `ENTREGADO`) agrupadas por el período solicitado.

### Query params

| Parámetro | Tipo   | Requerido | Default   | Valores válidos                      |
|-----------|--------|-----------|-----------|--------------------------------------|
| `periodo` | string | No        | `diario`  | `diario`, `mensual`, `trimestral`    |

### Período cubierto por valor

| Valor        | Datos incluidos              | Agrupación en resumen        |
|--------------|------------------------------|------------------------------|
| `diario`     | Últimos 30 días              | Por fecha (`dd/MM/yyyy`)     |
| `mensual`    | Últimos 12 meses             | Por mes (`yyyy-MM`)          |
| `trimestral` | Últimos 12 meses             | Por trimestre (`yyyy Q1..4`) |

### Ejemplos de uso

```
GET /api/Reportes/ventas
GET /api/Reportes/ventas?periodo=mensual
GET /api/Reportes/ventas?periodo=trimestral
```

### Response 200

```
Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
Content-Disposition: attachment; filename="ventas_mensual_20260701_1430.xlsx"
```

### Errores

| Código | Descripción                              |
|--------|------------------------------------------|
| `401`  | Token no enviado o expirado              |
| `403`  | El usuario autenticado no tiene rol ADMIN|

---

## Resumen de endpoints

| Método | Ruta                         | Descripción                        |
|--------|------------------------------|------------------------------------|
| `GET`  | `/api/Reportes/stock`        | Reporte de inventario por SKU      |
| `GET`  | `/api/Reportes/ventas`       | Reporte de ventas por período      |

---

## Contenido de los archivos

### Excel de Stock — 2 hojas

#### Hoja 1: Detalle de Stock

Tabla con una fila por combinación prenda-talla (SKU):

| # | Nombre Prenda | Categoría | Género | Marca | Talla | Stock | Estado |
|---|---------------|-----------|--------|-------|-------|-------|--------|
| 1 | Polo Clásico  | Tops      | Hombre | Nike  | M     | 12    | DISPONIBLE |
| 2 | Blusa Floral  | Blusas    | Mujer  | Zara  | S     | 3     | STOCK BAJO |
| 3 | Jogger Fit    | Pantalones| Mujer  | Adidas| XL    | 0     | SIN STOCK  |

**Código de colores por fila:**

| Color      | Condición              | Estado mostrado |
|------------|------------------------|-----------------|
| Rojo       | `stock = 0`            | SIN STOCK       |
| Amarillo   | `stock > 0` y `< 5`   | STOCK BAJO      |
| Azul claro | `stock >= 5` (pares)  | DISPONIBLE      |
| Blanco     | `stock >= 5` (impares) | DISPONIBLE      |

**Encabezado del reporte:** muestra fecha de generación, filtros aplicados y total de registros.

#### Hoja 2: Resumen

**KPIs (tarjetas verdes):**

| Indicador               | Valor |
|-------------------------|-------|
| Total SKUs (tallas)     | 48    |
| Total unidades en stock | 320   |
| SKUs sin stock          | 5     |
| SKUs con stock bajo     | 8     |

**Tabla: Stock por Categoría**

| Categoría   | Unidades | Nº SKUs | Sin Stock |
|-------------|----------|---------|-----------|
| Tops        | 120      | 18      | 1         |
| Pantalones  | 95       | 14      | 2         |
| Accesorios  | 45       | 8       | 0         |

**Tabla: Stock por Género**

| Género   | Unidades | Nº SKUs | Sin Stock |
|----------|----------|---------|-----------|
| Mujer    | 180      | 26      | 3         |
| Hombre   | 110      | 16      | 2         |
| Infantil | 30       | 6       | 0         |

---

### Excel de Ventas — 2 hojas

#### Hoja 1: Detalle de Ventas

Tabla con una fila por venta:

| # | Fecha      | N° Venta | Cliente     | Items | Estado    | Total (S/.) |
|---|------------|----------|-------------|-------|-----------|-------------|
| 1 | 01/07/2026 | 42       | juan.perez  | 3     | ENTREGADO | 285.00      |
| 2 | 01/07/2026 | 43       | maria.lopez | 1     | ENVIADO   | 95.00       |

- El color del texto en la columna **Estado** indica el estado: verde (ENTREGADO), azul (ENVIADO), oscuro (PAGADO).
- La fila **TOTAL GENERAL** al final suma todos los montos con fondo azul marino.

#### Hoja 2: Resumen por Período

**KPIs (tarjetas verdes):**

| Indicador         | Valor      |
|-------------------|------------|
| Total de ventas   | 22         |
| Monto total (S/.) | 2,090.00   |
| Ticket promedio   | 95.00      |

**Tabla de desglose** (varía según el parámetro `periodo`):

*Ejemplo `diario`:*

| Período    | Ventas | Monto (S/.) |
|------------|--------|-------------|
| 29/06/2026 | 5      | 475.00      |
| 30/06/2026 | 17     | 1,615.00    |
| **TOTAL**  | **22** | **2,090.00**|

*Ejemplo `mensual`:*

| Período | Ventas | Monto (S/.) |
|---------|--------|-------------|
| 2026-01 | 8      | 760.00      |
| 2026-06 | 14     | 1,330.00    |
| **TOTAL**| **22**| **2,090.00**|

*Ejemplo `trimestral`:*

| Período | Ventas | Monto (S/.) |
|---------|--------|-------------|
| 2026 Q1 | 8      | 760.00      |
| 2026 Q2 | 14     | 1,330.00    |
| **TOTAL**| **22**| **2,090.00**|
