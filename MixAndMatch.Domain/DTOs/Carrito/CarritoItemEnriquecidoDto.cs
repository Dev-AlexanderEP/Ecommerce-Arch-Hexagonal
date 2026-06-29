namespace MixAndMatch.Domain.DTOs.Carrito;

public class CarritoItemDetalladoDto
{
    public long Id { get; set; }
    public long CarritoId { get; set; }
    public long PrendaTallaId { get; set; }
    public long PrendaId { get; set; }
    public long TallaId { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public PrendaResumenCarritoDto Prenda { get; set; } = null!;
    public TallaResumenCarritoDto Talla { get; set; } = null!;
}

public class PrendaResumenCarritoDto
{
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public string? ImagenPrincipal { get; set; }
    public string NomMarca { get; set; } = null!;
}

public class TallaResumenCarritoDto
{
    public string NomTalla { get; set; } = null!;
}
