namespace MixAndMatch.Domain.DTOs.Envio;

public class EnvioTrackingResponseDto
{
    public long Id { get; set; }
    public string? TrackingNumber { get; set; }
    public string Estado { get; set; } = null!;
    public string MetodoEnvio { get; set; } = null!;
    public decimal CostoEnvio { get; set; }
    public DateOnly FechaEnvio { get; set; }
    public DateOnly? FechaEntrega { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DatosPersonalesDto DatosPersonales { get; set; } = null!;
    public VentaTrackingDto Venta { get; set; } = null!;
}

public class DatosPersonalesDto
{
    public long Id { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string Departamento { get; set; } = null!;
    public string Provincia { get; set; } = null!;
    public string Distrito { get; set; } = null!;
    public string? Calle { get; set; }
    public string Detalle { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class VentaTrackingDto
{
    public long Id { get; set; }
    public string? Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public List<DetalleTrackingDto> Detalles { get; set; } = [];
}

public class DetalleTrackingDto
{
    public long Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public PrendaTrackingDto Prenda { get; set; } = null!;
    public TallaTrackingDto Talla { get; set; } = null!;
}

public class PrendaTrackingDto
{
    public string Nombre { get; set; } = null!;
    public string? ImagenPrincipal { get; set; }
}

public class TallaTrackingDto
{
    public string NomTalla { get; set; } = null!;
}
