using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class FiltrarPrendasDinamicoQuery : IRequest<ApiResponse<List<PrendaConDescuentoResponseDto>>>
{
    public string? Talla { get; set; }
    public string? Categoria { get; set; }
    public string? Marca { get; set; }
    public string? Genero { get; set; }
    public double? PrecioMin { get; set; }
    public double? PrecioMax { get; set; }
    public double? DescMin { get; set; }
    public double? DescMax { get; set; }
}

public class FiltrarPrendasDinamicoQueryHandler(IUnitOfWork _uow) : IRequestHandler<FiltrarPrendasDinamicoQuery, ApiResponse<List<PrendaConDescuentoResponseDto>>>
{
    public async Task<ApiResponse<List<PrendaConDescuentoResponseDto>>> Handle(FiltrarPrendasDinamicoQuery request, CancellationToken ct)
    {
        var prendas = await _uow.Prendas.FiltrarDinamico(
            request.Talla, request.Categoria, request.Marca, request.Genero,
            request.PrecioMin, request.PrecioMax, request.DescMin, request.DescMax);
        return ApiResponse<List<PrendaConDescuentoResponseDto>>.Ok(prendas);
    }
}
