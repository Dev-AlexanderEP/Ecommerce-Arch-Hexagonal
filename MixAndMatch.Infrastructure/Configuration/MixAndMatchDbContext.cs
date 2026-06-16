using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Infrastructure.Configuration;

public partial class MixAndMatchDbContext : DbContext
{
    public MixAndMatchDbContext(DbContextOptions<MixAndMatchDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carrito> Carritos { get; set; }

    public virtual DbSet<CarritoItem> CarritoItems { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<DatosEnvio> DatosEnvios { get; set; }

    public virtual DbSet<DescuentoCategoria> DescuentoCategoria { get; set; }

    public virtual DbSet<DescuentoCodigo> DescuentoCodigos { get; set; }

    public virtual DbSet<DescuentoPrenda> DescuentoPrenda { get; set; }

    public virtual DbSet<DescuentoUsuario> DescuentoUsuarios { get; set; }

    public virtual DbSet<Envio> Envios { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PrendaImagen> PrendaImagens { get; set; }

    public virtual DbSet<PrendaTalla> PrendaTallas { get; set; }

    public virtual DbSet<Prenda> Prenda { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Resenia> Resenia { get; set; }

    public virtual DbSet<Talla> Tallas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    public virtual DbSet<VentasDetalle> VentasDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carrito_pkey");

            entity.ToTable("carrito");

            entity.HasIndex(e => e.UsuarioId, "uq_carrito_activo")
                .IsUnique()
                .HasFilter("((estado)::text = 'ACTIVO'::text)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithOne(p => p.Carrito)
                .HasForeignKey<Carrito>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carrito_usuario_id_fkey");
        });

        modelBuilder.Entity<CarritoItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carrito_item_pkey");

            entity.ToTable("carrito_item");

            entity.HasIndex(e => new { e.CarritoId, e.PrendaTallaId }, "carrito_item_carrito_id_prenda_talla_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CarritoId).HasColumnName("carrito_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.PrendaTallaId).HasColumnName("prenda_talla_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Carrito).WithMany(p => p.CarritoItems)
                .HasForeignKey(d => d.CarritoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carrito_item_carrito_id_fkey");

            entity.HasOne(d => d.PrendaTalla).WithMany(p => p.CarritoItems)
                .HasForeignKey(d => d.PrendaTallaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carrito_item_prenda_talla_id_fkey");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_pkey");

            entity.ToTable("categoria");

            entity.HasIndex(e => e.NomCategoria, "categoria_nom_categoria_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NomCategoria)
                .HasMaxLength(255)
                .HasColumnName("nom_categoria");
        });

        modelBuilder.Entity<DatosEnvio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("datos_envio_pkey");

            entity.ToTable("datos_envio");

            entity.HasIndex(e => e.UsuarioId, "uq_datos_envio_principal")
                .IsUnique()
                .HasFilter("(es_principal = true)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Calle)
                .HasMaxLength(255)
                .HasColumnName("calle");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Departamento)
                .HasMaxLength(100)
                .HasColumnName("departamento");
            entity.Property(e => e.Detalle)
                .HasMaxLength(255)
                .HasColumnName("detalle");
            entity.Property(e => e.Distrito)
                .HasMaxLength(100)
                .HasColumnName("distrito");
            entity.Property(e => e.Dni)
                .HasMaxLength(20)
                .HasColumnName("dni");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EsPrincipal)
                .HasDefaultValue(false)
                .HasColumnName("es_principal");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.Provincia)
                .HasMaxLength(100)
                .HasColumnName("provincia");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithOne(p => p.DatosEnvio)
                .HasForeignKey<DatosEnvio>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("datos_envio_usuario_id_fkey");
        });

        modelBuilder.Entity<DescuentoCategoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("descuento_categoria_pkey");

            entity.ToTable("descuento_categoria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Categoria).WithMany(p => p.DescuentoCategoria)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("descuento_categoria_categoria_id_fkey");
        });

        modelBuilder.Entity<DescuentoCodigo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("descuento_codigo_pkey");

            entity.ToTable("descuento_codigo");

            entity.HasIndex(e => e.Codigo, "descuento_codigo_codigo_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .HasColumnName("codigo");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsoMaximo).HasColumnName("uso_maximo");
        });

        modelBuilder.Entity<DescuentoPrenda>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("descuento_prenda_pkey");

            entity.ToTable("descuento_prenda");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje");
            entity.Property(e => e.PrendaId).HasColumnName("prenda_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Prenda).WithMany(p => p.DescuentoPrenda)
                .HasForeignKey(d => d.PrendaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("descuento_prenda_prenda_id_fkey");
        });

        modelBuilder.Entity<DescuentoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("descuento_usuario_pkey");

            entity.ToTable("descuento_usuario");

            entity.HasIndex(e => new { e.DescuentoCodigoId, e.UsuarioId }, "descuento_usuario_descuento_codigo_id_usuario_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DescuentoCodigoId).HasColumnName("descuento_codigo_id");
            entity.Property(e => e.FechaUso).HasColumnName("fecha_uso");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.DescuentoCodigo).WithMany(p => p.DescuentoUsuarios)
                .HasForeignKey(d => d.DescuentoCodigoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("descuento_usuario_descuento_codigo_id_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.DescuentoUsuarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("descuento_usuario_usuario_id_fkey");
        });

        modelBuilder.Entity<Envio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("envio_pkey");

            entity.ToTable("envio");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CostoEnvio)
                .HasPrecision(10, 2)
                .HasColumnName("costo_envio");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DatosEnvioId).HasColumnName("datos_envio_id");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.FechaEntrega).HasColumnName("fecha_entrega");
            entity.Property(e => e.FechaEnvio).HasColumnName("fecha_envio");
            entity.Property(e => e.MetodoEnvio)
                .HasMaxLength(50)
                .HasColumnName("metodo_envio");
            entity.Property(e => e.TrackingNumber)
                .HasMaxLength(100)
                .HasColumnName("tracking_number");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VentaId).HasColumnName("venta_id");

            entity.HasOne(d => d.DatosEnvio).WithMany(p => p.Envios)
                .HasForeignKey(d => d.DatosEnvioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("envio_datos_envio_id_fkey");

            entity.HasOne(d => d.Venta).WithMany(p => p.Envios)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("envio_venta_id_fkey");
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genero_pkey");

            entity.ToTable("genero");

            entity.HasIndex(e => e.NomGenero, "genero_nom_genero_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NomGenero)
                .HasMaxLength(100)
                .HasColumnName("nom_genero");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("marca_pkey");

            entity.ToTable("marca");

            entity.HasIndex(e => e.NomMarca, "marca_nom_marca_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NomMarca)
                .HasMaxLength(255)
                .HasColumnName("nom_marca");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("metodo_pago_pkey");

            entity.ToTable("metodo_pago");

            entity.HasIndex(e => e.TipoPago, "metodo_pago_tipo_pago_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(50)
                .HasColumnName("tipo_pago");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pago_pkey");

            entity.ToTable("pago");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.MetodoId).HasColumnName("metodo_id");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VentaId).HasColumnName("venta_id");

            entity.HasOne(d => d.Metodo).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.MetodoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_metodo_id_fkey");

            entity.HasOne(d => d.Venta).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_venta_id_fkey");
        });

        modelBuilder.Entity<PrendaImagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prenda_imagen_pkey");

            entity.ToTable("prenda_imagen");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");
            entity.Property(e => e.PrendaId).HasColumnName("prenda_id");
            entity.Property(e => e.Tipo)
                .HasColumnName("tipo");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .HasColumnName("url");

            entity.HasOne(d => d.Prenda).WithMany(p => p.PrendaImagens)
                .HasForeignKey(d => d.PrendaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prenda_imagen_prenda_id_fkey");
        });

        modelBuilder.Entity<PrendaTalla>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prenda_talla_pkey");

            entity.ToTable("prenda_talla");

            entity.HasIndex(e => new { e.PrendaId, e.TallaId }, "prenda_talla_prenda_id_talla_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PrendaId).HasColumnName("prenda_id");
            entity.Property(e => e.Stock)
                .HasDefaultValue(0)
                .HasColumnName("stock");
            entity.Property(e => e.TallaId).HasColumnName("talla_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Prenda).WithMany(p => p.PrendaTallas)
                .HasForeignKey(d => d.PrendaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prenda_talla_prenda_id_fkey");

            entity.HasOne(d => d.Talla).WithMany(p => p.PrendaTallas)
                .HasForeignKey(d => d.TallaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prenda_talla_talla_id_fkey");
        });

        modelBuilder.Entity<Prenda>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prenda_pkey");

            entity.ToTable("prenda");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.GeneroId).HasColumnName("genero_id");
            entity.Property(e => e.MarcaId).HasColumnName("marca_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.ProveedorId).HasColumnName("proveedor_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Prenda)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prenda_categoria_id_fkey");

            entity.HasOne(d => d.Genero).WithMany(p => p.Prenda)
                .HasForeignKey(d => d.GeneroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prenda_genero_id_fkey");

            entity.HasOne(d => d.Marca).WithMany(p => p.Prenda)
                .HasForeignKey(d => d.MarcaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prenda_marca_id_fkey");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Prenda)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prenda_proveedor_id_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proveedor_pkey");

            entity.ToTable("proveedor");

            entity.HasIndex(e => e.NomProveedor, "proveedor_nom_proveedor_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NomProveedor)
                .HasMaxLength(255)
                .HasColumnName("nom_proveedor");
        });

        modelBuilder.Entity<Resenia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("resenia_pkey");

            entity.ToTable("resenia");

            entity.HasIndex(e => new { e.PrendaId, e.UsuarioId }, "resenia_prenda_id_usuario_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.Comentario).HasColumnName("comentario");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PrendaId).HasColumnName("prenda_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Prenda).WithMany(p => p.Resenia)
                .HasForeignKey(d => d.PrendaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resenia_prenda_id_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Resenia)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resenia_usuario_id_fkey");
        });

        modelBuilder.Entity<Talla>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("talla_pkey");

            entity.ToTable("talla");

            entity.HasIndex(e => e.NomTalla, "talla_nom_talla_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NomTalla)
                .HasMaxLength(20)
                .HasColumnName("nom_talla");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Email, "usuarios_email_key").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "usuarios_nombre_usuario_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Contrasenia)
                .HasMaxLength(255)
                .HasColumnName("contrasenia");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(255)
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.Rol)
                .HasColumnName("rol");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ventas_pkey");

            entity.ToTable("ventas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Venta)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ventas_usuario_id_fkey");
        });

        modelBuilder.Entity<VentasDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ventas_detalle_pkey");

            entity.ToTable("ventas_detalle");

            entity.HasIndex(e => new { e.VentaId, e.PrendaTallaId }, "ventas_detalle_venta_id_prenda_talla_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.PrendaTallaId).HasColumnName("prenda_talla_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VentaId).HasColumnName("venta_id");

            entity.HasOne(d => d.PrendaTalla).WithMany(p => p.VentasDetalles)
                .HasForeignKey(d => d.PrendaTallaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ventas_detalle_prenda_talla_id_fkey");

            entity.HasOne(d => d.Venta).WithMany(p => p.VentasDetalles)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ventas_detalle_venta_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
