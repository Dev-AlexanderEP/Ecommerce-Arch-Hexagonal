using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetEstadisticasPreciosPorCategoriaQuery : IRequest<ApiResponse<PrendaPrecioStatsDto>>
{
    public required string Categoria { get; set; }
}

public class GetEstadisticasPreciosPorCategoriaQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetEstadisticasPreciosPorCategoriaQuery, ApiResponse<PrendaPrecioStatsDto>>
{
    public async Task<ApiResponse<PrendaPrecioStatsDto>> Handle(GetEstadisticasPreciosPorCategoriaQuery request, CancellationToken ct)
    {
        var stats = await _uow.Prendas.BuscarEstadisticasPreciosPorCategoria(request.Categoria);
        return ApiResponse<PrendaPrecioStatsDto>.Ok(stats);
    }
}
