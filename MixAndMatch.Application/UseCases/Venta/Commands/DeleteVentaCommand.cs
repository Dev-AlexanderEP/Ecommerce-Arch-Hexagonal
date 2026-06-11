using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class DeleteVentaCommand : IRequest<ApiResponse<bool>>
{
    public required long VentaId { get; set; }
}

public class DeleteVentaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteVentaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteVentaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<VentaEntity>();
        var entity = await repo.GetById(request.VentaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        var detalles = await _uow.Repository<VentasDetalleEntity>().GetAll();
        if (detalles.Any(x => x.VentaId == request.VentaId))
        {
            return ApiResponse<bool>.Fail("La venta tiene detalles asociados y no puede eliminarse.");
        }

        await repo.Delete(request.VentaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Venta eliminada correctamente.");
    }
}
