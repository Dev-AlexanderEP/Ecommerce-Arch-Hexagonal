using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetTallasPorGeneroQuery : IRequest<ApiResponse<List<string>>>
{
    public required string Genero { get; set; }
}

public class GetTallasPorGeneroQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetTallasPorGeneroQuery, ApiResponse<List<string>>>
{
    public async Task<ApiResponse<List<string>>> Handle(GetTallasPorGeneroQuery request, CancellationToken ct)
    {
        var tallas = await _uow.Prendas.BuscarTallasPorGenero(request.Genero);
        return ApiResponse<List<string>>.Ok(tallas);
    }
}
