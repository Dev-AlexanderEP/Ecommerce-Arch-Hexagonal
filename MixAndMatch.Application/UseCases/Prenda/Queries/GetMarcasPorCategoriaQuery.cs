using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetMarcasPorCategoriaQuery : IRequest<ApiResponse<List<string>>>
{
    public required string Categoria { get; set; }
}

public class GetMarcasPorCategoriaQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetMarcasPorCategoriaQuery, ApiResponse<List<string>>>
{
    public async Task<ApiResponse<List<string>>> Handle(GetMarcasPorCategoriaQuery request, CancellationToken ct)
    {
        var marcas = await _uow.Prendas.BuscarMarcasPorCategoria(request.Categoria);
        return ApiResponse<List<string>>.Ok(marcas);
    }
}
