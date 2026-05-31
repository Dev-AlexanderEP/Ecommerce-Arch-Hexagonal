using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class UpdatePrendaTallaCommand : IRequest<ApiResponseDto<PrendaTallaResponseDto>>
{
    public required long PrendaTallaId { get; set; }
    public int Stock { get; set; }
}

public class UpdatePrendaTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdatePrendaTallaCommand, ApiResponseDto<PrendaTallaResponseDto>>
{
    public async Task<ApiResponseDto<PrendaTallaResponseDto>> Handle(UpdatePrendaTallaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaTallaEntity>();
        var entity = await repo.GetById(request.PrendaTallaId);
        if (entity is null)
            return ApiResponseDto<PrendaTallaResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");

        if (request.Stock < 0)
            return ApiResponseDto<PrendaTallaResponseDto>.Fail("El stock no puede ser negativo.");

        entity.Stock = request.Stock;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

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
