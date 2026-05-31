using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.DTOs.Resenias;

public class ReseniaResponseDto
{
    public long Id { get; set; }

    public long PrendaId { get; set; }

    public long UsuarioId { get; set; }

    public int Calificacion { get; set; }

    public string? Comentario { get; set; }

    public EstadoResenia Estado { get; set; }

    public Guid? ModeradoPorId { get; set; }

    public DateTime? ModeradoEn { get; set; }

    public string? MotivoRechazo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
