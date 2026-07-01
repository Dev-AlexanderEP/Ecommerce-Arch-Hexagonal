using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetVentasPorGeneroQuery : IRequest<ApiResponse<List<VentasPorGeneroDto>>> { }

public class GetVentasPorGeneroQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetVentasPorGeneroQuery, ApiResponse<List<VentasPorGeneroDto>>>
{
    public async Task<ApiResponse<List<VentasPorGeneroDto>>> Handle(GetVentasPorGeneroQuery request, CancellationToken ct)
        => ApiResponse<List<VentasPorGeneroDto>>.Ok(await _uow.Ventas.GetVentasPorGenero());
}
