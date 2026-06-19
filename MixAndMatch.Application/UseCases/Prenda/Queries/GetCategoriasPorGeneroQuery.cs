using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetCategoriasPorGeneroQuery : IRequest<ApiResponse<List<string>>>
{
    public required string Genero { get; set; }
}

public class GetCategoriasPorGeneroQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetCategoriasPorGeneroQuery, ApiResponse<List<string>>>
{
    public async Task<ApiResponse<List<string>>> Handle(GetCategoriasPorGeneroQuery request, CancellationToken ct)
    {
        var categorias = await _uow.Prendas.BuscarCategoriasPorGenero(request.Genero);
        return ApiResponse<List<string>>.Ok(categorias);
    }
}
