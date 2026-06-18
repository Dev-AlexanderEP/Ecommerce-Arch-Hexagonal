using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class DeleteCarritoItemCommand : IRequest<ApiResponse<bool>>
{
    public required long CarritoItemId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class DeleteCarritoItemCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteCarritoItemCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCarritoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.CarritoItems.GetById(request.CarritoItemId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"CarritoItem no encontrado para id {request.CarritoItemId}.");
        }

        var carrito = await _uow.Carritos.GetById(entity.CarritoId);
        if (carrito is null || carrito.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<bool>.Fail("No tienes acceso a este item.", ErrorType.Forbidden);
        }

        await _uow.CarritoItems.Delete(request.CarritoItemId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "CarritoItem eliminado correctamente.");
    }
}
