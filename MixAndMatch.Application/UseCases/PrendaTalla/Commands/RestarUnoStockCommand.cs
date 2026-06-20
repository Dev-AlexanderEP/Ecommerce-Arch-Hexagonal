using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class RestarUnoStockCommand : IRequest<ApiResponse<PrendaTallaResponseDto>>
{
    public required long PrendaId { get; set; }
    public required long TallaId { get; set; }
}

public class RestarUnoStockCommandHandler(IUnitOfWork _uow) : IRequestHandler<RestarUnoStockCommand, ApiResponse<PrendaTallaResponseDto>>
{
    public async Task<ApiResponse<PrendaTallaResponseDto>> Handle(RestarUnoStockCommand request, CancellationToken ct)
    {
        var filas = await _uow.PrendaTallas.RestarUnoStock(request.PrendaId, request.TallaId);
        if (filas == 0)
            return ApiResponse<PrendaTallaResponseDto>.Fail("Stock insuficiente o combinación no encontrada.", ErrorType.Validation);

        var entity = await _uow.PrendaTallas.BuscarPorPrendaYTalla(request.PrendaId, request.TallaId);
        return ApiResponse<PrendaTallaResponseDto>.Ok(new PrendaTallaResponseDto
        {
            Id = entity!.Id,
            PrendaId = entity.PrendaId,
            TallaId = entity.TallaId,
            Stock = entity.Stock,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
