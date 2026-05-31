using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Queries;

public class GetPrendaTallaByIdQuery : IRequest<ApiResponseDto<PrendaTallaResponseDto>>
{
    public required long PrendaTallaId { get; set; }
}

public class GetPrendaTallaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetPrendaTallaByIdQuery, ApiResponseDto<PrendaTallaResponseDto>>
{
    public async Task<ApiResponseDto<PrendaTallaResponseDto>> Handle(GetPrendaTallaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<PrendaTallaEntity>().GetById(request.PrendaTallaId);
        if (entity is null)
            return ApiResponseDto<PrendaTallaResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");

        return ApiResponseDto<PrendaTallaResponseDto>.Ok(new PrendaTallaResponseDto
        {
            Id = entity.Id,
            PrendaId = entity.PrendaId,
            TallaId = entity.TallaId,
            Stock = entity.Stock,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
