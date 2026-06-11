using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Queries;

public class GetVentasDetalleByIdQuery : IRequest<ApiResponse<VentasDetalleResponseDto>>
{
    public required long VentasDetalleId { get; set; }
}

public class GetVentasDetalleByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetVentasDetalleByIdQuery, ApiResponse<VentasDetalleResponseDto>>
{
    public async Task<ApiResponse<VentasDetalleResponseDto>> Handle(GetVentasDetalleByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<VentasDetalleEntity>().GetById(request.VentasDetalleId);
        if (entity is null)
        {
            return ApiResponse<VentasDetalleResponseDto>.Fail($"VentasDetalle no encontrado para id {request.VentasDetalleId}.");
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
