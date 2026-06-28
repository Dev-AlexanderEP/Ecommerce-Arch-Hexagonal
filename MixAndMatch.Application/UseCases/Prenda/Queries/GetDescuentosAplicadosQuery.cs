using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetDescuentosAplicadosQuery : IRequest<ApiResponse<List<PrendaConDescuentoResponseDto>>>
{
    public string? Categoria { get; set; }
    public string? Genero { get; set; }
}

public class GetDescuentosAplicadosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentosAplicadosQuery, ApiResponse<List<PrendaConDescuentoResponseDto>>>
{
    public async Task<ApiResponse<List<PrendaConDescuentoResponseDto>>> Handle(GetDescuentosAplicadosQuery request, CancellationToken ct)
    {
        var prendas = await _uow.Prendas.BuscarDescuentosAplicados(request.Categoria, request.Genero);
        return ApiResponse<List<PrendaConDescuentoResponseDto>>.Ok(prendas);
    }
}
