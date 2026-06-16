using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Carrito.Commands;

public class DeleteCarritoCommand : IRequest<ApiResponse<bool>>
{
    public required long CarritoId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class DeleteCarritoCommandHandler(ICarritoRepository _carritos, IUnitOfWork _uow) : IRequestHandler<DeleteCarritoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCarritoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _carritos.GetById(request.CarritoId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        if (entity.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<bool>.Fail("No tienes acceso a este carrito.", ErrorType.Forbidden);
        }

        if (await _carritos.TieneItems(request.CarritoId))
        {
            return ApiResponse<bool>.Fail("El carrito tiene ítems asociados y no puede eliminarse.", ErrorType.Conflict);
        }

        await _carritos.Delete(request.CarritoId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Carrito eliminado correctamente.");
    }
}
