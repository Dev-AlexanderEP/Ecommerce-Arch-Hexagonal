using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class DeleteVentaCommand : IRequest<ApiResponse<bool>>
{
    public required long VentaId { get; set; }
}

public class DeleteVentaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteVentaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVentaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Ventas.GetById(request.VentaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        if (await _uow.Ventas.TieneDetalles(request.VentaId))
        {
            return ApiResponse<bool>.Fail("La venta tiene detalles asociados y no puede eliminarse.", ErrorType.Conflict);
        }

        if (await _uow.Ventas.TienePagos(request.VentaId))
        {
            return ApiResponse<bool>.Fail("La venta tiene pagos asociados y no puede eliminarse.", ErrorType.Conflict);
        }

        if (await _uow.Ventas.TieneEnvios(request.VentaId))
        {
            return ApiResponse<bool>.Fail("La venta tiene envíos asociados y no puede eliminarse.", ErrorType.Conflict);
        }

        await _uow.Ventas.Delete(request.VentaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Venta eliminada correctamente.");
    }
}
