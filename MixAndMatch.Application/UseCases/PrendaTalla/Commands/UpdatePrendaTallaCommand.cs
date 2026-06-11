using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class UpdatePrendaTallaCommand : IRequest<ApiResponse<PrendaTallaResponseDto>>
{
    public required long PrendaTallaId { get; set; }
    public int Stock { get; set; }
}

public class UpdatePrendaTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdatePrendaTallaCommand, ApiResponse<PrendaTallaResponseDto>>
{
    public async Task<ApiResponse<PrendaTallaResponseDto>> Handle(UpdatePrendaTallaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaTallaEntity>();
        var entity = await repo.GetById(request.PrendaTallaId);
        if (entity is null)
            return ApiResponse<PrendaTallaResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");

        if (request.Stock < 0)
            return ApiResponse<PrendaTallaResponseDto>.Fail("El stock no puede ser negativo.");

        entity.Stock = request.Stock;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
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
