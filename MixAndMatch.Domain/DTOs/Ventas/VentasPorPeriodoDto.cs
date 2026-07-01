namespace MixAndMatch.Domain.DTOs.Ventas;

public class VentasPorPeriodoDto
{
    public string Periodo { get; set; } = string.Empty;
    public int CantidadVentas { get; set; }
}

public class VentasPorGeneroDto
{
    public string Genero { get; set; } = string.Empty;
    public int CantidadVentas { get; set; }
}

public class ResumenPrendasDto
{
    public int Activas { get; set; }
    public int Inactivas { get; set; }
}
