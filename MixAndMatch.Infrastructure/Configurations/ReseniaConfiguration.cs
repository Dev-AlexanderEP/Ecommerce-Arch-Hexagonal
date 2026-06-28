using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Infrastructure.Configurations;

public sealed class ReseniaConfiguration : IEntityTypeConfiguration<Resenia>
{
    public void Configure(EntityTypeBuilder<Resenia> builder)
    {
        builder.ToTable("resenia");

        builder.HasKey(r => r.Id).HasName("resenia_pkey");

        builder.HasIndex(r => new { r.PrendaId, r.UsuarioId })
            .IsUnique()
            .HasDatabaseName("resenia_prenda_id_usuario_id_key");

        builder.Property(r => r.Id).HasColumnName("id");
        builder.Property(r => r.PrendaId).HasColumnName("prenda_id");
        builder.Property(r => r.UsuarioId).HasColumnName("usuario_id");
        builder.Property(r => r.Comentario).HasColumnName("comentario");
        builder.Property(r => r.Estado)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasColumnName("estado");
        builder.Property(r => r.ModeradoPorId).HasColumnName("moderado_por_id");
        builder.Property(r => r.ModeradoEn)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("moderado_en");
        builder.Property(r => r.MotivoRechazo).HasColumnName("motivo_rechazo");
        builder.Property(r => r.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(r => r.Calificacion)
            .HasColumnName("calificacion")
            .HasColumnType("integer");

        builder.HasOne(r => r.Prenda).WithMany(p => p.Resenia)
            .HasForeignKey(r => r.PrendaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("resenia_prenda_id_fkey");

        builder.HasOne(r => r.Usuario).WithMany(p => p.Resenia)
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("resenia_usuario_id_fkey");

        builder.HasOne(r => r.ModeradoPor).WithMany()
            .HasForeignKey(r => r.ModeradoPorId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("resenia_moderado_por_id_fkey");
    }
}
