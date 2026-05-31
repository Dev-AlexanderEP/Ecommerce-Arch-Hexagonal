namespace MixAndMatch.Domain.DTOs.MetodoPago;

public class MetodoPagoResponseDto
{
    public long Id { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}