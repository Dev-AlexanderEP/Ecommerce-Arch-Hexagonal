using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetEstadisticasPreciosPorGeneroQuery : IRequest<ApiResponse<PrendaPrecioStatsDto>>
{
    public required string Genero { get; set; }
}

public class GetEstadisticasPreciosPorGeneroQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetEstadisticasPreciosPorGeneroQuery, ApiResponse<PrendaPrecioStatsDto>>
{
    public async Task<ApiResponse<PrendaPrecioStatsDto>> Handle(GetEstadisticasPreciosPorGeneroQuery request, CancellationToken ct)
    {
        var stats = await _uow.Prendas.BuscarEstadisticasPreciosPorGenero(request.Genero);
        return ApiResponse<PrendaPrecioStatsDto>.Ok(stats);
    }
}
