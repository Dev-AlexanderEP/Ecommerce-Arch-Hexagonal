using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Queries;

public class GetPrendaTallaByIdQuery : IRequest<ApiResponse<PrendaTallaResponseDto>>
{
    public required long PrendaTallaId { get; set; }
}

public class GetPrendaTallaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetPrendaTallaByIdQuery, ApiResponse<PrendaTallaResponseDto>>
{
    public async Task<ApiResponse<PrendaTallaResponseDto>> Handle(GetPrendaTallaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.PrendaTallas.GetById(request.PrendaTallaId);
        if (entity is null)
        {
            return ApiResponse<PrendaTallaResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");
        }

        return ApiResponse<PrendaTallaResponseDto>.Ok(new PrendaTallaResponseDto
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
