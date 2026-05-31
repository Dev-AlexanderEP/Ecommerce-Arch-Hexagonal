namespace MixAndMatch.Domain.DTOs.Descuentos;

public class DescuentoUsuarioResponseDto
{
    public long Id { get; set; }

    public long DescuentoCodigoId { get; set; }

    public long UsuarioId { get; set; }

    public DateOnly FechaUso { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
