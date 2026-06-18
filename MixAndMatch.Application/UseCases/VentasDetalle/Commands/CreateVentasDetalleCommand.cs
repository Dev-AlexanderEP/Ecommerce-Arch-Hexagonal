using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Commands;

public class CreateVentasDetalleCommand : IRequest<ApiResponse<VentasDetalleResponseDto>>
{
    public required long VentaId { get; set; }
    public required long PrendaTallaId { get; set; }
    public required int Cantidad { get; set; }
    public required decimal PrecioUnitario { get; set; }
}

public class CreateVentasDetalleCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateVentasDetalleCommand, ApiResponse<VentasDetalleResponseDto>>
{
    public async Task<ApiResponse<VentasDetalleResponseDto>> Handle(CreateVentasDetalleCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Ventas.GetById(request.VentaId) is null)
        {
            return ApiResponse<VentasDetalleResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.Validation);
        }

        if (await _uow.PrendaTallas.GetById(request.PrendaTallaId) is null)
        {
            return ApiResponse<VentasDetalleResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.", ErrorType.Validation);
        }

        if (await _uow.VentasDetalles.ExisteEnVenta(request.VentaId, request.PrendaTallaId))
        {
            return ApiResponse<VentasDetalleResponseDto>.Fail("Ya existe un detalle con esa combinación de venta y prenda-talla.", ErrorType.Conflict);
        }

        var entity = new VentasDetalleEntity
        {
            VentaId = request.VentaId,
            PrendaTallaId = request.PrendaTallaId,
            Cantidad = request.Cantidad,
            PrecioUnitario = request.PrecioUnitario,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.VentasDetalles.Add(entity);
        await _uow.Complete();

        return ApiResponse<VentasDetalleResponseDto>.Created(new VentasDetalleResponseDto
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
