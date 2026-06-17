using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Queries;

public class GetAllVentasDetallesQuery : IRequest<ApiPaginationResponse<VentasDetalleResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllVentasDetallesQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllVentasDetallesQuery, ApiPaginationResponse<VentasDetalleResponseDto>>
{
    public async Task<ApiPaginationResponse<VentasDetalleResponseDto>> Handle(GetAllVentasDetallesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.VentasDetalles.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<VentasDetalleResponseDto>.Ok(items.Select(x => new VentasDetalleResponseDto
        {
            Id = x.Id,
            VentaId = x.VentaId,
            PrendaTallaId = x.PrendaTallaId,
            Cantidad = x.Cantidad,
            PrecioUnitario = x.PrecioUnitario,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
