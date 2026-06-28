using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetDescuentosAplicadosAleatorioQuery : IRequest<ApiResponse<List<PrendaConDescuentoTodoResponseDto>>>
{
    public required string Genero { get; set; }
}

public class GetDescuentosAplicadosAleatorioQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentosAplicadosAleatorioQuery, ApiResponse<List<PrendaConDescuentoTodoResponseDto>>>
{
    public async Task<ApiResponse<List<PrendaConDescuentoTodoResponseDto>>> Handle(GetDescuentosAplicadosAleatorioQuery request, CancellationToken ct)
    {
        var prendas = await _uow.Prendas.BuscarDescuentosAplicadosAleatorio(request.Genero);
        return ApiResponse<List<PrendaConDescuentoTodoResponseDto>>.Ok(prendas);
    }
}
