namespace MixAndMatch.Domain.DTOs.Ventas;

public class VentaResponseDto
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? Estado { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
