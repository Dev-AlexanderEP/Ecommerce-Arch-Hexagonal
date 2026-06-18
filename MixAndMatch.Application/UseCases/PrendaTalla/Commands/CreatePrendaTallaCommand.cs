using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class CreatePrendaTallaCommand : IRequest<ApiResponse<PrendaTallaResponseDto>>
{
    public required long PrendaId { get; set; }
    public required long TallaId { get; set; }
    public int Stock { get; set; }
}

public class CreatePrendaTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreatePrendaTallaCommand, ApiResponse<PrendaTallaResponseDto>>
{
    public async Task<ApiResponse<PrendaTallaResponseDto>> Handle(CreatePrendaTallaCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Prendas.GetById(request.PrendaId) is null)
        {
            return ApiResponse<PrendaTallaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.", ErrorType.Validation);
        }

        if (await _uow.Tallas.GetById(request.TallaId) is null)
        {
            return ApiResponse<PrendaTallaResponseDto>.Fail($"Talla no encontrada para id {request.TallaId}.", ErrorType.Validation);
        }

        if (await _uow.PrendaTallas.ExisteCombinacion(request.PrendaId, request.TallaId))
        {
            return ApiResponse<PrendaTallaResponseDto>.Fail("Ya existe una combinación de prenda y talla con esos valores.", ErrorType.Conflict);
        }

        var entity = new PrendaTallaEntity
        {
            PrendaId = request.PrendaId,
            TallaId = request.TallaId,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.PrendaTallas.Add(entity);
        await _uow.Complete();

        return ApiResponse<PrendaTallaResponseDto>.Created(new PrendaTallaResponseDto
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
