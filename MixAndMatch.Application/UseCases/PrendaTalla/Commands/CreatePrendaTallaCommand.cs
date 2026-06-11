using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

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
        var prenda = await _uow.Repository<PrendaEntity>().GetById(request.PrendaId);
        if (prenda is null)
            return ApiResponse<PrendaTallaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");

        var talla = await _uow.Repository<TallaEntity>().GetById(request.TallaId);
        if (talla is null)
            return ApiResponse<PrendaTallaResponseDto>.Fail($"Talla no encontrada para id {request.TallaId}.");

        if (request.Stock < 0)
            return ApiResponse<PrendaTallaResponseDto>.Fail("El stock no puede ser negativo.");

        var existentes = await _uow.Repository<PrendaTallaEntity>().GetAll();
        if (existentes.Any(x => x.PrendaId == request.PrendaId && x.TallaId == request.TallaId))
            return ApiResponse<PrendaTallaResponseDto>.Fail("Ya existe una combinaciÃ³n de prenda y talla con esos valores.");

        var entity = new PrendaTallaEntity
        {
            PrendaId = request.PrendaId,
            TallaId = request.TallaId,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<PrendaTallaEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponse<PrendaTallaResponseDto>.Ok(ToDto(entity));
    }

    private static PrendaTallaResponseDto ToDto(PrendaTallaEntity e) => new()
    {
        Id = e.Id,
        PrendaId = e.PrendaId,
        TallaId = e.TallaId,
        Stock = e.Stock,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
