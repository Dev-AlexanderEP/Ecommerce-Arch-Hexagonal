using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class BuscarPrendasConDescuentoQuery : IRequest<ApiResponse<List<PrendaConDescuentoResponseDto>>>
{
    public string? Nombre { get; set; }
    public string? Categoria { get; set; }
    public string? Genero { get; set; }
}

public class BuscarPrendasConDescuentoQueryHandler(IUnitOfWork _uow) : IRequestHandler<BuscarPrendasConDescuentoQuery, ApiResponse<List<PrendaConDescuentoResponseDto>>>
{
    public async Task<ApiResponse<List<PrendaConDescuentoResponseDto>>> Handle(BuscarPrendasConDescuentoQuery request, CancellationToken ct)
    {
        var prendas = await _uow.Prendas.BuscarPrendasConDescuento(request.Nombre, request.Categoria, request.Genero);
        return ApiResponse<List<PrendaConDescuentoResponseDto>>.Ok(prendas);
    }
}
