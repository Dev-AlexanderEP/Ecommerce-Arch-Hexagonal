# Módulo de Analíticas — Dashboard Admin

Endpoints de solo lectura para alimentar el panel de administración: tarjetas de resumen, gráfico de barras de ventas por período y gráfico de torta por género.

Base URL: `/api`

> Todos los endpoints requieren `Authorization: Bearer <token>` con rol **ADMIN**.

---

## Tabla de contenidos

1. [Cards de resumen](#1-cards-de-resumen)
   - [Total de ventas realizadas](#total-de-ventas-realizadas)
   - [Total de usuarios registrados](#total-de-usuarios-registrados)
   - [Resumen de prendas](#resumen-de-prendas)
2. [Gráfico de barras — Ventas por período](#2-gráfico-de-barras--ventas-por-período)
3. [Gráfico de torta — Ventas por género](#3-gráfico-de-torta--ventas-por-género)

---

## 1. Cards de resumen

### Total de ventas realizadas

```
GET /api/Ventas/total
```

Cuenta las ventas con estado `PAGADO`, `ENVIADO` o `ENTREGADO`.

**Response 200:**
```json
{
  "success": true,
  "data": 350
}
```

---

### Total de usuarios registrados

```
GET /api/Usuarios/total
```

Cuenta todos los usuarios sin importar estado o rol.

**Response 200:**
```json
{
  "success": true,
  "data": 120
}
```

---

### Resumen de prendas

```
GET /api/Prendas/resumen
```

Devuelve el conteo de prendas activas e inactivas.

**Response 200:**
```json
{
  "success": true,
  "data": {
    "activas": 80,
    "inactivas": 15
  }
}
```

| Campo      | Tipo | Descripción                    |
|------------|------|--------------------------------|
| `activas`  | int  | Prendas con `activo = true`    |
| `inactivas`| int  | Prendas con `activo = false`   |

---

## 2. Gráfico de barras — Ventas por período

```
GET /api/Ventas/por-periodo?agrupacion={valor}
```

Devuelve el conteo de ventas realizadas agrupadas por el período solicitado. Solo incluye ventas con estado `PAGADO`, `ENVIADO` o `ENTREGADO`.

**Query params:**

| Parámetro    | Tipo   | Requerido | Default   |
|--------------|--------|-----------|-----------|
| `agrupacion` | string | No        | `diario`  |

**Valores de `agrupacion`:**

| Valor      | Período cubierto         | Formato de `periodo`   | Ejemplo       |
|------------|--------------------------|------------------------|---------------|
| `diario`   | Últimos 30 días por fecha| `yyyy-MM-dd`           | `"2026-06-30"`|
| `semanal`  | Todo el historial por semana del año | `yyyy-SNN` | `"2026-S26"`  |
| `mensual`  | Todo el historial por mes | `yyyy-MM`             | `"2026-06"`   |
| `anual`    | Todo el historial por año | `yyyy`                | `"2026"`      |

**Ejemplos de uso:**
```
GET /api/Ventas/por-periodo                         → diario (últimos 30 días)
GET /api/Ventas/por-periodo?agrupacion=semanal
GET /api/Ventas/por-periodo?agrupacion=mensual
GET /api/Ventas/por-periodo?agrupacion=anual
```

**Response 200 (ejemplo diario):**
```json
{
  "success": true,
  "data": [
    { "periodo": "2026-06-24", "cantidadVentas": 5 },
    { "periodo": "2026-06-25", "cantidadVentas": 12 },
    { "periodo": "2026-06-26", "cantidadVentas": 8 },
    { "periodo": "2026-06-27", "cantidadVentas": 15 },
    { "periodo": "2026-06-28", "cantidadVentas": 20 },
    { "periodo": "2026-06-29", "cantidadVentas": 7 },
    { "periodo": "2026-06-30", "cantidadVentas": 3 }
  ]
}
```

**Response 200 (ejemplo mensual):**
```json
{
  "success": true,
  "data": [
    { "periodo": "2026-01", "cantidadVentas": 95 },
    { "periodo": "2026-02", "cantidadVentas": 110 },
    { "periodo": "2026-03", "cantidadVentas": 145 }
  ]
}
```

**Item del array:**

| Campo           | Tipo   | Descripción                           |
|-----------------|--------|---------------------------------------|
| `periodo`       | string | Etiqueta del período (ver tabla arriba)|
| `cantidadVentas`| int    | Número de ventas en ese período       |

> La lista viene ordenada por `periodo` ascendente, lista para mapear directamente al eje X del gráfico.

---

## 3. Gráfico de torta — Ventas por género

```
GET /api/Ventas/por-genero
```

Agrupa las unidades vendidas por el género de la prenda (Mujer, Hombre, Accesorios, Infantil, etc.). Solo incluye ventas con estado `PAGADO`, `ENVIADO` o `ENTREGADO`.

**Response 200:**
```json
{
  "success": true,
  "data": [
    { "genero": "Mujer",      "cantidadVentas": 210 },
    { "genero": "Hombre",     "cantidadVentas": 95  },
    { "genero": "Accesorios", "cantidadVentas": 30  },
    { "genero": "Infantil",   "cantidadVentas": 15  }
  ]
}
```

**Item del array:**

| Campo           | Tipo   | Descripción                                    |
|-----------------|--------|------------------------------------------------|
| `genero`        | string | Nombre del género (`NomGenero`)                |
| `cantidadVentas`| int    | Total de unidades vendidas (suma de `Cantidad` en detalles) |

> Ordenado por `cantidadVentas` descendente. El género con más ventas aparece primero.

---

## Resumen de endpoints

| Método | Ruta                             | Descripción                            |
|--------|----------------------------------|----------------------------------------|
| `GET`  | `/api/Ventas/total`              | Total de ventas realizadas (card)      |
| `GET`  | `/api/Ventas/por-periodo`        | Ventas por período (gráfico de barras) |
| `GET`  | `/api/Ventas/por-genero`         | Ventas por género (gráfico de torta)   |
| `GET`  | `/api/Usuarios/total`            | Total de usuarios registrados (card)   |
| `GET`  | `/api/Prendas/resumen`           | Prendas activas e inactivas (card)     |

---

## Ejemplo de integración — Dashboard

```
// 1. Cards
GET /api/Ventas/total       → data: 350   (Ventas realizadas)
GET /api/Usuarios/total     → data: 120   (Usuarios registrados)
GET /api/Prendas/resumen    → { activas: 80, inactivas: 15 }

// 2. Gráfico de barras (al cambiar el filtro)
GET /api/Ventas/por-periodo?agrupacion=diario
GET /api/Ventas/por-periodo?agrupacion=semanal
GET /api/Ventas/por-periodo?agrupacion=mensual
GET /api/Ventas/por-periodo?agrupacion=anual

// 3. Gráfico de torta
GET /api/Ventas/por-genero
```
