namespace MixAndMatch.Domain.DTOs.Resenias;

public class ReseniaByPrendaItemDto
{
    public long UsuarioId { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public int PuntajeEstrellas { get; set; }

    public int CantidadResenias { get; set; }

    public string? Comentario { get; set; }

    public DateTime CreatedAt { get; set; }
}
