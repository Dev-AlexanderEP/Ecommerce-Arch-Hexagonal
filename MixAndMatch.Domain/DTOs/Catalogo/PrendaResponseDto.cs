namespace MixAndMatch.Domain.DTOs;

public class PrendaResponseDto
{
    public long Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public long MarcaId { get; set; }

    public long CategoriaId { get; set; }

    public long ProveedorId { get; set; }

    public long GeneroId { get; set; }

    public decimal Precio { get; set; }

    public bool Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
