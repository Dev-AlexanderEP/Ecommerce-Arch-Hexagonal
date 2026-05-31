namespace MixAndMatch.Domain.DTOs;

public class CarritoItemResponseDto
{
    public long Id { get; set; }
    public long CarritoId { get; set; }
    public long PrendaTallaId { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
