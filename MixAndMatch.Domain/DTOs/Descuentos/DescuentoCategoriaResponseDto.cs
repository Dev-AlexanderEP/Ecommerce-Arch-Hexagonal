namespace MixAndMatch.Domain.DTOs.Descuentos;

public class DescuentoCategoriaResponseDto
{
    public long Id { get; set; }

    public long CategoriaId { get; set; }

    public decimal Porcentaje { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
