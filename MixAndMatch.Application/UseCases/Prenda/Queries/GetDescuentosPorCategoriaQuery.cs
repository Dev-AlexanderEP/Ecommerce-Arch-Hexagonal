using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetDescuentosPorCategoriaQuery : IRequest<ApiResponse<List<decimal>>>
{
    public required string Categoria { get; set; }
}

public class GetDescuentosPorCategoriaQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentosPorCategoriaQuery, ApiResponse<List<decimal>>>
{
    public async Task<ApiResponse<List<decimal>>> Handle(GetDescuentosPorCategoriaQuery request, CancellationToken ct)
    {
        var descuentos = await _uow.Prendas.BuscarDescuentosPorCategoria(request.Categoria);
        return ApiResponse<List<decimal>>.Ok(descuentos);
    }
}
