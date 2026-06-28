namespace MixAndMatch.Domain.Entities;

public partial class Resenia
{
    public long Id { get; set; }

    public long PrendaId { get; set; }

    public long UsuarioId { get; set; }

    public int Calificacion { get; set; }

    public string? Comentario { get; set; }

    public EstadoResenia Estado { get; set; }

    public long? ModeradoPorId { get; set; }

    public virtual Usuario? ModeradoPor { get; set; }

    public DateTime? ModeradoEn { get; set; }

    public string? MotivoRechazo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Prenda Prenda { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
