using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Commands;

public class DeleteVentasDetalleCommand : IRequest<ApiResponse<bool>>
{
    public required long VentasDetalleId { get; set; }
}

public class DeleteVentasDetalleCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteVentasDetalleCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVentasDetalleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.VentasDetalles.GetById(request.VentasDetalleId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"VentasDetalle no encontrado para id {request.VentasDetalleId}.");
        }

        await _uow.VentasDetalles.Delete(request.VentasDetalleId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "VentasDetalle eliminado correctamente.");
    }
}
