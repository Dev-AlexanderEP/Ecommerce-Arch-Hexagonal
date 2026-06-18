using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Commands;

public class DeletePrendaTallaCommand : IRequest<ApiResponse<bool>>
{
    public required long PrendaTallaId { get; set; }
}

public class DeletePrendaTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeletePrendaTallaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeletePrendaTallaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.PrendaTallas.GetById(request.PrendaTallaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");
        }

        if (await _uow.PrendaTallas.TieneItemsCarrito(request.PrendaTallaId))
        {
            return ApiResponse<bool>.Fail("La combinación de prenda y talla tiene ítems de carrito asociados.", ErrorType.Conflict);
        }

        if (await _uow.PrendaTallas.TieneVentas(request.PrendaTallaId))
        {
            return ApiResponse<bool>.Fail("La combinación de prenda y talla tiene ventas asociadas.", ErrorType.Conflict);
        }

        await _uow.PrendaTallas.Delete(request.PrendaTallaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "PrendaTalla eliminada correctamente.");
    }
}
