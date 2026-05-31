namespace MixAndMatch.Domain.DTOs;

public class EnvioResponseDto
{
    public long Id { get; set; }

    public long VentaId { get; set; }

    public long DatosEnvioId { get; set; }

    public decimal CostoEnvio { get; set; }

    public DateOnly FechaEnvio { get; set; }

    public DateOnly? FechaEntrega { get; set; }

    public string Estado { get; set; } = null!;

    public string MetodoEnvio { get; set; } = null!;

    public string? TrackingNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}