-- ============================================================
--  TIENDA DE ROPA — Schema completo
--  Base: mixandmatch | PostgreSQL 17
-- ============================================================

-- ─── ENUMS ──────────────────────────────────────────────────

CREATE TYPE rol_usuario      AS ENUM ('ADMIN', 'CLIENTE', 'VENDEDOR');
CREATE TYPE tipo_imagen      AS ENUM ('PRINCIPAL', 'HOVER', 'SECUNDARIA', 'TERCIARIA', 'DETALLE');
CREATE TYPE estado_carrito   AS ENUM ('ACTIVO', 'CERRADO', 'ABANDONADO');
CREATE TYPE estado_venta     AS ENUM ('PENDIENTE', 'PAGADO', 'ENVIADO', 'ENTREGADO', 'CANCELADO');
CREATE TYPE estado_pago      AS ENUM ('PENDIENTE', 'COMPLETADO', 'FALLIDO', 'REEMBOLSADO');
CREATE TYPE estado_envio     AS ENUM ('PREPARANDO', 'EN_CAMINO', 'ENTREGADO', 'DEVUELTO');

-- ─── IDENTITY ───────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS usuarios (
    id             BIGSERIAL    PRIMARY KEY NOT NULL,
    nombre_usuario VARCHAR(255) NOT NULL UNIQUE,
    email          VARCHAR(255) NOT NULL UNIQUE,
    contrasenia    VARCHAR(255),
    rol            rol_usuario,
    activo         BOOLEAN      DEFAULT true,
    created_at     TIMESTAMPTZ,
    updated_at     TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS datos_envio (
    id           BIGSERIAL    PRIMARY KEY NOT NULL,
    usuario_id   BIGINT       NOT NULL REFERENCES usuarios(id),
    nombres      VARCHAR(100) NOT NULL,
    apellidos    VARCHAR(100) NOT NULL,
    dni          VARCHAR(20)  NOT NULL,
    departamento VARCHAR(100) NOT NULL,
    provincia    VARCHAR(100) NOT NULL,
    distrito     VARCHAR(100) NOT NULL,
    calle        VARCHAR(255),
    detalle      VARCHAR(255) NOT NULL,
    telefono     VARCHAR(20)  NOT NULL,
    email        VARCHAR(100) NOT NULL,
    es_principal BOOLEAN      DEFAULT false NOT NULL,
    created_at   TIMESTAMPTZ   NOT NULL,
    updated_at   TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS uq_datos_envio_principal
    ON datos_envio (usuario_id)
    WHERE es_principal = true;

-- ─── CATALOG ────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS categoria (
    id            BIGSERIAL    PRIMARY KEY NOT NULL,
    nom_categoria VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS genero (
    id         BIGSERIAL    PRIMARY KEY NOT NULL,
    nom_genero VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS marca (
    id        BIGSERIAL    PRIMARY KEY NOT NULL,
    nom_marca VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS proveedor (
    id            BIGSERIAL    PRIMARY KEY NOT NULL,
    nom_proveedor VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS talla (
    id        BIGSERIAL   PRIMARY KEY NOT NULL,
    nom_talla VARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS prenda (
    id           BIGSERIAL     PRIMARY KEY NOT NULL,
    nombre       VARCHAR(255)  NOT NULL,
    descripcion  TEXT,
    marca_id     BIGINT        NOT NULL REFERENCES marca(id),
    categoria_id BIGINT        NOT NULL REFERENCES categoria(id),
    proveedor_id BIGINT        NOT NULL REFERENCES proveedor(id),
    genero_id    BIGINT        NOT NULL REFERENCES genero(id),
    precio       DECIMAL(10,2) NOT NULL,
    activo       BOOLEAN       DEFAULT true NOT NULL,
    created_at   TIMESTAMPTZ,
    updated_at   TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS prenda_imagen (
    id         BIGSERIAL    PRIMARY KEY NOT NULL,
    prenda_id  BIGINT       NOT NULL REFERENCES prenda(id),
    tipo       tipo_imagen  NOT NULL,
    url        VARCHAR(500) NOT NULL,
    orden      INT          DEFAULT 0,
    created_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS prenda_talla (
    id         BIGSERIAL PRIMARY KEY NOT NULL,
    prenda_id  BIGINT    NOT NULL REFERENCES prenda(id),
    talla_id   BIGINT    NOT NULL REFERENCES talla(id),
    stock      INT       NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ,
    UNIQUE (prenda_id, talla_id)
);

-- ─── CART ───────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS carrito (
    id             BIGSERIAL   PRIMARY KEY NOT NULL,
    usuario_id     BIGINT      NOT NULL REFERENCES usuarios(id),
    fecha_creacion TIMESTAMPTZ,
    estado         estado_carrito,
    updated_at     TIMESTAMPTZ
);

CREATE UNIQUE INDEX IF NOT EXISTS uq_carrito_activo
    ON carrito (usuario_id)
    WHERE estado = 'ACTIVO';

CREATE TABLE IF NOT EXISTS carrito_item (
    id              BIGSERIAL     PRIMARY KEY NOT NULL,
    carrito_id      BIGINT        NOT NULL REFERENCES carrito(id),
    prenda_talla_id BIGINT        NOT NULL REFERENCES prenda_talla(id),
    precio_unitario DECIMAL(10,2) NOT NULL,
    cantidad        INT           NOT NULL,
    created_at      TIMESTAMPTZ,
    updated_at      TIMESTAMPTZ,
    UNIQUE (carrito_id, prenda_talla_id)
);

-- ─── ORDERS ─────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS ventas (
    id             BIGSERIAL   PRIMARY KEY NOT NULL,
    usuario_id     BIGINT      NOT NULL REFERENCES usuarios(id),
    fecha_creacion TIMESTAMPTZ  NOT NULL,
    estado         estado_venta,
    updated_at     TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS ventas_detalle (
    id              BIGSERIAL     PRIMARY KEY NOT NULL,
    venta_id        BIGINT        NOT NULL REFERENCES ventas(id),
    prenda_talla_id BIGINT        NOT NULL REFERENCES prenda_talla(id),
    cantidad        INT           NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    created_at      TIMESTAMPTZ,
    updated_at      TIMESTAMPTZ,
    UNIQUE (venta_id, prenda_talla_id)
);

-- ─── PAYMENT ────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS metodo_pago (
    id         BIGSERIAL   PRIMARY KEY NOT NULL,
    tipo_pago  VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS pago (
    id             BIGSERIAL     PRIMARY KEY NOT NULL,
    venta_id       BIGINT        NOT NULL REFERENCES ventas(id),
    metodo_id      BIGINT        NOT NULL REFERENCES metodo_pago(id),
    monto          DECIMAL(10,2) NOT NULL,
    estado         estado_pago   NOT NULL,
    fecha_creacion TIMESTAMPTZ    NOT NULL,
    updated_at     TIMESTAMPTZ
);

-- ─── SHIPPING ───────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS envio (
    id              BIGSERIAL     PRIMARY KEY NOT NULL,
    venta_id        BIGINT        NOT NULL REFERENCES ventas(id),
    datos_envio_id  BIGINT        NOT NULL REFERENCES datos_envio(id),
    costo_envio     DECIMAL(10,2) NOT NULL,
    fecha_envio     DATE          NOT NULL,
    fecha_entrega   DATE,
    estado          estado_envio  NOT NULL,
    metodo_envio    VARCHAR(50)   NOT NULL,
    tracking_number VARCHAR(100),
    created_at      TIMESTAMPTZ    NOT NULL,
    updated_at      TIMESTAMPTZ
);

-- ─── DISCOUNTS ──────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS descuento_prenda (
    id           BIGSERIAL    PRIMARY KEY NOT NULL,
    prenda_id    BIGINT       NOT NULL REFERENCES prenda(id),
    porcentaje   DECIMAL(5,2) NOT NULL,
    fecha_inicio DATE         NOT NULL,
    fecha_fin    DATE,
    activo       BOOLEAN      NOT NULL,
    created_at   TIMESTAMPTZ,
    updated_at   TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS descuento_categoria (
    id           BIGSERIAL    PRIMARY KEY NOT NULL,
    categoria_id BIGINT       NOT NULL REFERENCES categoria(id),
    porcentaje   DECIMAL(5,2) NOT NULL,
    fecha_inicio DATE         NOT NULL,
    fecha_fin    DATE,
    activo       BOOLEAN      NOT NULL,
    created_at   TIMESTAMPTZ,
    updated_at   TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS descuento_codigo (
    id           BIGSERIAL    PRIMARY KEY NOT NULL,
    codigo       VARCHAR(50)  NOT NULL UNIQUE,
    descripcion  VARCHAR(255),
    porcentaje   DECIMAL(5,2) NOT NULL,
    fecha_inicio DATE         NOT NULL,
    fecha_fin    DATE,
    uso_maximo   INT          NOT NULL,
    activo       BOOLEAN      NOT NULL,
    created_at   TIMESTAMPTZ,
    updated_at   TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS descuento_usuario (
    id                  BIGSERIAL PRIMARY KEY NOT NULL,
    descuento_codigo_id BIGINT    NOT NULL REFERENCES descuento_codigo(id),
    usuario_id          BIGINT    NOT NULL REFERENCES usuarios(id),
    fecha_uso           DATE      NOT NULL,
    created_at          TIMESTAMPTZ,
    updated_at          TIMESTAMPTZ,
    UNIQUE (descuento_codigo_id, usuario_id)
);

-- ─── REVIEWS ────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS "resenia" (
    id           BIGSERIAL PRIMARY KEY NOT NULL,
    prenda_id    BIGINT    NOT NULL REFERENCES prenda(id),
    usuario_id   BIGINT    NOT NULL REFERENCES usuarios(id),
    calificacion INT       NOT NULL,
    comentario   TEXT,
    estado       VARCHAR(50) NOT NULL,
    moderado_por_id BIGINT REFERENCES usuarios(id),
    moderado_en  TIMESTAMPTZ,
    motivo_rechazo VARCHAR(255),
    created_at   TIMESTAMPTZNOT NULL,
    updated_at   TIMESTAMPTZ,
    UNIQUE (prenda_id, usuario_id)
);
