namespace MixAndMatch.Domain.DTOs;

public class PrendaConDescuentoResponseDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string? ImagenPrincipal { get; set; }
    public string? ImagenHover { get; set; }
    public string Marca { get; set; } = string.Empty;
    public decimal DescuentoAplicado { get; set; }
    public string TipoDescuento { get; set; } = string.Empty;
}
