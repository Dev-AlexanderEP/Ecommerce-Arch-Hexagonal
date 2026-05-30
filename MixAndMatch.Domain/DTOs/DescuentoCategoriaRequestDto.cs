namespace MixAndMatch.Domain.DTOs;

public class DescuentoCategoriaRequestDto
{
    public long CategoriaId { get; set; }

    public decimal Porcentaje { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; }
}
