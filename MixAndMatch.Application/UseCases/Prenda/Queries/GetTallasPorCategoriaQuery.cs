using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetTallasPorCategoriaQuery : IRequest<ApiResponse<List<string>>>
{
    public required string Categoria { get; set; }
}

public class GetTallasPorCategoriaQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetTallasPorCategoriaQuery, ApiResponse<List<string>>>
{
    public async Task<ApiResponse<List<string>>> Handle(GetTallasPorCategoriaQuery request, CancellationToken ct)
    {
        var tallas = await _uow.Prendas.BuscarTallasPorCategoria(request.Categoria);
        return ApiResponse<List<string>>.Ok(tallas);
    }
}
