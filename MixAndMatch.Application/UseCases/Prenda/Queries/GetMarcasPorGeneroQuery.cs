using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetMarcasPorGeneroQuery : IRequest<ApiResponse<List<string>>>
{
    public required string Genero { get; set; }
}

public class GetMarcasPorGeneroQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetMarcasPorGeneroQuery, ApiResponse<List<string>>>
{
    public async Task<ApiResponse<List<string>>> Handle(GetMarcasPorGeneroQuery request, CancellationToken ct)
    {
        var marcas = await _uow.Prendas.BuscarMarcasPorGenero(request.Genero);
        return ApiResponse<List<string>>.Ok(marcas);
    }
}
