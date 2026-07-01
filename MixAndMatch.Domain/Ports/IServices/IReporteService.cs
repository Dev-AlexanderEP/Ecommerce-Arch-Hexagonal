namespace MixAndMatch.Domain.Ports.IServices;

public interface IReporteService
{
    Task<byte[]> GenerarReporteStock(string? nombre, string? genero, string? categoria);
    Task<byte[]> GenerarReporteVentas(string periodo);
}
