using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class DeletePrendaTallaCommand : IRequest<ApiResponse<bool>>
{
    public required long PrendaTallaId { get; set; }
}

public class DeletePrendaTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeletePrendaTallaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeletePrendaTallaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaTallaEntity>();
        var entity = await repo.GetById(request.PrendaTallaId);
        if (entity is null)
            return ApiResponse<bool>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");

        var carritoItems = await _uow.Repository<CarritoItemEntity>().GetAll();
        if (carritoItems.Any(x => x.PrendaTallaId == request.PrendaTallaId))
            return ApiResponse<bool>.Fail("La combinaciÃ³n de prenda y talla tiene Ã­tems de carrito asociados.");

        var ventasDetalles = await _uow.Repository<VentasDetalleEntity>().GetAll();
        if (ventasDetalles.Any(x => x.PrendaTallaId == request.PrendaTallaId))
            return ApiResponse<bool>.Fail("La combinaciÃ³n de prenda y talla tiene ventas asociadas.");

        await repo.Delete(request.PrendaTallaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "PrendaTalla eliminada correctamente.");
    }
}
