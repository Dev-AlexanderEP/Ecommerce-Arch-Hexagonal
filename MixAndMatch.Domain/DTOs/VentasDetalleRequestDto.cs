namespace MixAndMatch.Domain.DTOs;

public class VentasDetalleRequestDto
{
    public long VentaId { get; set; }
    public long PrendaTallaId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
