namespace MixAndMatch.Domain.DTOs;

public class CarritoItemRequestDto
{
    public long CarritoId { get; set; }
    public long PrendaTallaId { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}
