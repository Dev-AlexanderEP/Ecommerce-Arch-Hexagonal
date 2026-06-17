using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class UpdatePrendaTallaCommand : IRequest<ApiResponse<PrendaTallaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long PrendaTallaId { get; set; }
    public int Stock { get; set; }
}

public class UpdatePrendaTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdatePrendaTallaCommand, ApiResponse<PrendaTallaResponseDto>>
{
    public async Task<ApiResponse<PrendaTallaResponseDto>> Handle(UpdatePrendaTallaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.PrendaTallas.GetById(request.PrendaTallaId);
        if (entity is null)
        {
            return ApiResponse<PrendaTallaResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");
        }

        entity.Stock = request.Stock;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.PrendaTallas.Update(entity);
        await _uow.Complete();

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
