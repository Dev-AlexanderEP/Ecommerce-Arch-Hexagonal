using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Talla.Queries;

public class GetTallaPorNombreQuery : IRequest<ApiResponse<TallaResponseDto>>
{
    public required string NomTalla { get; set; }
}

public class GetTallaPorNombreQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetTallaPorNombreQuery, ApiResponse<TallaResponseDto>>
{
    public async Task<ApiResponse<TallaResponseDto>> Handle(GetTallaPorNombreQuery request, CancellationToken ct)
    {
        var entity = await _uow.Tallas.BuscarPorNombre(request.NomTalla);
        if (entity is null)
            return ApiResponse<TallaResponseDto>.Fail($"Talla no encontrada para nombre {request.NomTalla}.");

        return ApiResponse<TallaResponseDto>.Ok(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
