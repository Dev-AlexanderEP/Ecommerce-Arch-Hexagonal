using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetTotalVentasQuery : IRequest<ApiResponse<int>> { }

public class GetTotalVentasQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetTotalVentasQuery, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(GetTotalVentasQuery request, CancellationToken ct)
        => ApiResponse<int>.Ok(await _uow.Ventas.GetTotalRealizadas());
}
