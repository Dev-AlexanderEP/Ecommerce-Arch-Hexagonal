namespace MixAndMatch.Domain.DTOs;

public class PrendaRequestDto
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public long MarcaId { get; set; }

    public long CategoriaId { get; set; }

    public long ProveedorId { get; set; }

    public long GeneroId { get; set; }

    public decimal Precio { get; set; }

    public bool Activo { get; set; }
}
