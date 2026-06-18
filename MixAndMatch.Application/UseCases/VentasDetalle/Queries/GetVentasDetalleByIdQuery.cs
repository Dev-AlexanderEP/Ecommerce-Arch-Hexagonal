using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Queries;

public class GetVentasDetalleByIdQuery : IRequest<ApiResponse<VentasDetalleResponseDto>>
{
    public required long VentasDetalleId { get; set; }
    public required long SolicitanteId { get; set; }
    public required bool EsAdmin { get; set; }
}

public class GetVentasDetalleByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetVentasDetalleByIdQuery, ApiResponse<VentasDetalleResponseDto>>
{
    public async Task<ApiResponse<VentasDetalleResponseDto>> Handle(GetVentasDetalleByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.VentasDetalles.GetById(request.VentasDetalleId);
        if (entity is null)
        {
            return ApiResponse<VentasDetalleResponseDto>.Fail($"VentasDetalle no encontrado para id {request.VentasDetalleId}.");
        }

        // El ADMIN ve cualquier detalle; el CLIENTE solo los de sus propias ventas.
        if (!request.EsAdmin)
        {
            var venta = await _uow.Ventas.GetById(entity.VentaId);
            if (venta is null || venta.UsuarioId != request.SolicitanteId)
            {
                return ApiResponse<VentasDetalleResponseDto>.Fail("No tienes acceso a este detalle de venta.", ErrorType.Forbidden);
            }
        }

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
