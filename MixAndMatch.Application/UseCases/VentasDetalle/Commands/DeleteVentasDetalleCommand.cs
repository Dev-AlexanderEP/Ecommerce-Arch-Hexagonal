using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Commands;

public class DeleteVentasDetalleCommand : IRequest<ApiResponseDto<bool>>
{
    public required long VentasDetalleId { get; set; }
}

public class DeleteVentasDetalleCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteVentasDetalleCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteVentasDetalleCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<VentasDetalleEntity>();
        var entity = await repo.GetById(request.VentasDetalleId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"VentasDetalle no encontrado para id {request.VentasDetalleId}.");
        }

        await repo.Delete(request.VentasDetalleId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "VentasDetalle eliminado correctamente.");
    }
}
