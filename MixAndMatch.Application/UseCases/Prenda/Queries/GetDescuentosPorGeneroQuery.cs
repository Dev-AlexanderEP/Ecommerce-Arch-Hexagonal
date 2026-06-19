using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetDescuentosPorGeneroQuery : IRequest<ApiResponse<List<decimal>>>
{
    public required string Genero { get; set; }
}

public class GetDescuentosPorGeneroQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentosPorGeneroQuery, ApiResponse<List<decimal>>>
{
    public async Task<ApiResponse<List<decimal>>> Handle(GetDescuentosPorGeneroQuery request, CancellationToken ct)
    {
        var descuentos = await _uow.Prendas.BuscarDescuentosPorGenero(request.Genero);
        return ApiResponse<List<decimal>>.Ok(descuentos);
    }
}
