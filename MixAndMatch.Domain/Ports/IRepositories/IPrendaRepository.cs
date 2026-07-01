using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IPrendaRepository : IGenericRepository<Prenda>
{
    Task<bool> TieneDescuentos(long prendaId);
    Task<bool> TieneImagenes(long prendaId);
    Task<bool> TieneTallas(long prendaId);
    Task<bool> TieneResenias(long prendaId);

    Task<List<string>> BuscarTallasPorCategoria(string categoria);
    Task<List<string>> BuscarTallasPorGenero(string genero);
    Task<List<string>> BuscarMarcasPorCategoria(string categoria);
    Task<List<string>> BuscarMarcasPorGenero(string genero);
    Task<PrendaPrecioStatsDto> BuscarEstadisticasPreciosPorCategoria(string categoria);
    Task<PrendaPrecioStatsDto> BuscarEstadisticasPreciosPorGenero(string genero);
    Task<List<decimal>> BuscarDescuentosPorCategoria(string categoria);
    Task<List<decimal>> BuscarDescuentosPorGenero(string genero);
    Task<List<string>> BuscarCategoriasPorGenero(string genero);
    Task<List<PrendaConDescuentoResponseDto>> BuscarPrendasConDescuento(string? nombre, string? categoria, string? genero);
    Task<List<PrendaConDescuentoResponseDto>> BuscarDescuentosAplicados(string? categoria, string? genero);
    Task<List<PrendaConDescuentoTodoResponseDto>> BuscarDescuentosAplicadosAleatorio(string genero);
    Task<List<PrendaConDescuentoResponseDto>> FiltrarDinamico(string? talla, string? categoria, string? marca, string? genero, double? precioMin, double? precioMax, double? descMin, double? descMax);
    Task<Prenda?> GetDetalladoById(long id);

    Task<ResumenPrendasDto> GetResumen();
}
