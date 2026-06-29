namespace MixAndMatch.Domain.DTOs.Catalogo;

public class PrendaDetalladaResponseDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ImagenPrincipal { get; set; }
    public string? ImagenHover { get; set; }
    public string? ImagenVideo { get; set; }
    public string? ImagenExtra1 { get; set; }
    public string? ImagenExtra2 { get; set; }
    public MarcaResumenDto Marca { get; set; } = null!;
    public CategoriaResumenDto Categoria { get; set; } = null!;
    public ProveedorResumenDto Proveedor { get; set; } = null!;
    public List<TallaStockDto> Tallas { get; set; } = [];
}

public class MarcaResumenDto
{
    public long Id { get; set; }
    public string NomMarca { get; set; } = null!;
}

public class CategoriaResumenDto
{
    public long Id { get; set; }
    public string NomCategoria { get; set; } = null!;
}

public class ProveedorResumenDto
{
    public long Id { get; set; }
    public string NomProveedor { get; set; } = null!;
}

public class TallaStockDto
{
    public long PrendaTallaId { get; set; }
    public long TallaId { get; set; }
    public string NomTalla { get; set; } = null!;
    public int Stock { get; set; }
}
