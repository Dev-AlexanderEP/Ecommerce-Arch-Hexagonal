namespace MixAndMatch.Domain.DTOs.Descuentos;

public class DescuentoCodigoResponseDto
{
    public long Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Porcentaje { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public int UsoMaximo { get; set; }

    public bool Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
