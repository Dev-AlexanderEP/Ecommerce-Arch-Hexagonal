using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;

namespace MixAndMatch.Application.UseCases.Genero.Queries;

public class GetGeneroByIdQuery : IRequest<ApiResponseDto<GeneroResponseDto>>
{
    public required long GeneroId { get; set; }
}

public class GetGeneroByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetGeneroByIdQuery, ApiResponseDto<GeneroResponseDto>>
{
    public async Task<ApiResponseDto<GeneroResponseDto>> Handle(GetGeneroByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<GeneroEntity>().GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponseDto<GeneroResponseDto>.Fail($"Género no encontrado para id {request.GeneroId}.");
        }

        return ApiResponseDto<GeneroResponseDto>.Ok(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
