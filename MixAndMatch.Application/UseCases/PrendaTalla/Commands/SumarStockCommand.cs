using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class SumarStockCommand : IRequest<ApiResponse<PrendaTallaResponseDto>>
{
    public required long PrendaId { get; set; }
    public required long TallaId { get; set; }
    public required int Cantidad { get; set; }
}

public class SumarStockCommandHandler(IUnitOfWork _uow) : IRequestHandler<SumarStockCommand, ApiResponse<PrendaTallaResponseDto>>
{
    public async Task<ApiResponse<PrendaTallaResponseDto>> Handle(SumarStockCommand request, CancellationToken ct)
    {
        var filas = await _uow.PrendaTallas.SumarStock(request.PrendaId, request.TallaId, request.Cantidad);
        if (filas == 0)
            return ApiResponse<PrendaTallaResponseDto>.Fail("Combinación prenda-talla no encontrada.", ErrorType.NotFound);

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
