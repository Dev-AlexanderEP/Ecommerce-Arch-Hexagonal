using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Commands;

public class UpdateVentasDetalleCommand : IRequest<ApiResponseDto<VentasDetalleResponseDto>>
{
    public required long VentasDetalleId { get; set; }
    public required int Cantidad { get; set; }
    public required decimal PrecioUnitario { get; set; }
}

public class UpdateVentasDetalleCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateVentasDetalleCommand, ApiResponseDto<VentasDetalleResponseDto>>
{
    public async Task<ApiResponseDto<VentasDetalleResponseDto>> Handle(UpdateVentasDetalleCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<VentasDetalleEntity>();
        var entity = await repo.GetById(request.VentasDetalleId);
        if (entity is null)
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail($"VentasDetalle no encontrado para id {request.VentasDetalleId}.");
        }

        if (request.Cantidad <= 0)
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail("La cantidad debe ser mayor a 0.");
        }

        if (request.PrecioUnitario <= 0)
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail("El precio unitario debe ser mayor a 0.");
        }

        entity.Cantidad = request.Cantidad;
        entity.PrecioUnitario = request.PrecioUnitario;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponseDto<VentasDetalleResponseDto>.Ok(new VentasDetalleResponseDto
        {
            Id = entity.Id,
            VentaId = entity.VentaId,
            PrendaTallaId = entity.PrendaTallaId,
            Cantidad = entity.Cantidad,
            PrecioUnitario = entity.PrecioUnitario,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
