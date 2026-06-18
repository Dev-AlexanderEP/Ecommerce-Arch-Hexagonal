using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Genero.Queries;

public class GetGeneroByIdQuery : IRequest<ApiResponse<GeneroResponseDto>>
{
    public required long GeneroId { get; set; }
}

public class GetGeneroByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetGeneroByIdQuery, ApiResponse<GeneroResponseDto>>
{
    public async Task<ApiResponse<GeneroResponseDto>> Handle(GetGeneroByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Generos.GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponse<GeneroResponseDto>.Fail($"Género no encontrado para id {request.GeneroId}.");
        }

        return ApiResponse<GeneroResponseDto>.Ok(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
