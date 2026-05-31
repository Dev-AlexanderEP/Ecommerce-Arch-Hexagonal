using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class DeleteVentaCommand : IRequest<ApiResponseDto<bool>>
{
    public required long VentaId { get; set; }
}

public class DeleteVentaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteVentaCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteVentaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<VentaEntity>();
        var entity = await repo.GetById(request.VentaId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        var detalles = await _uow.Repository<VentasDetalleEntity>().GetAll();
        if (detalles.Any(x => x.VentaId == request.VentaId))
        {
            return ApiResponseDto<bool>.Fail("La venta tiene detalles asociados y no puede eliminarse.");
        }

        await repo.Delete(request.VentaId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Venta eliminada correctamente.");
    }
}
