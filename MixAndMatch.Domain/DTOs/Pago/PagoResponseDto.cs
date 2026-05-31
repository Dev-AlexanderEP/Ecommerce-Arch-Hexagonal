namespace MixAndMatch.Domain.DTOs.MetodoPago;

public class PagoResponseDto
{
    public long Id { get; set; }
    public long VentaId { get; set; }
    public long MetodoId { get; set; }
    public decimal Monto { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? UpdatedAt { get; set; }
}