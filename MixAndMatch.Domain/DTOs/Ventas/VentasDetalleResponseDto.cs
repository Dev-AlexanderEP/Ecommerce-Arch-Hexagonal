namespace MixAndMatch.Domain.DTOs.Ventas;

public class VentasDetalleResponseDto
{
    public long Id { get; set; }
    public long VentaId { get; set; }
    public long PrendaTallaId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
