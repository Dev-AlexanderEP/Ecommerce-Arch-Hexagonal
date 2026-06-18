using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Commands;

public class UpdateVentasDetalleCommand : IRequest<ApiResponse<VentasDetalleResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long VentasDetalleId { get; set; }
    public required int Cantidad { get; set; }
    public required decimal PrecioUnitario { get; set; }
}

public class UpdateVentasDetalleCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateVentasDetalleCommand, ApiResponse<VentasDetalleResponseDto>>
{
    public async Task<ApiResponse<VentasDetalleResponseDto>> Handle(UpdateVentasDetalleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.VentasDetalles.GetById(request.VentasDetalleId);
        if (entity is null)
        {
            return ApiResponse<VentasDetalleResponseDto>.Fail($"VentasDetalle no encontrado para id {request.VentasDetalleId}.");
        }

        entity.Cantidad = request.Cantidad;
        entity.PrecioUnitario = request.PrecioUnitario;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.VentasDetalles.Update(entity);
        await _uow.Complete();

        return ApiResponse<VentasDetalleResponseDto>.Ok(new VentasDetalleResponseDto
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
