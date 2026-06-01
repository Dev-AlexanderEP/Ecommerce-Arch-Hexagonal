using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Queries;

public class GetAllVentasDetallesQuery : IRequest<ApiResponseDto<IEnumerable<VentasDetalleResponseDto>>>
{
}

public class GetAllVentasDetallesQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllVentasDetallesQuery, ApiResponseDto<IEnumerable<VentasDetalleResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<VentasDetalleResponseDto>>> Handle(GetAllVentasDetallesQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<VentasDetalleEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<VentasDetalleResponseDto>>.Fail("No se encontraron detalles de venta.");
        }

        return ApiResponseDto<IEnumerable<VentasDetalleResponseDto>>.Ok(items.Select(x => new VentasDetalleResponseDto
        {
            Id = x.Id,
            VentaId = x.VentaId,
            PrendaTallaId = x.PrendaTallaId,
            Cantidad = x.Cantidad,
            PrecioUnitario = x.PrecioUnitario,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
