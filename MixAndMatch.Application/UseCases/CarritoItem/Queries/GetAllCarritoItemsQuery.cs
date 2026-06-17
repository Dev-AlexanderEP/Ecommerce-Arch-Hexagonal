using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.CarritoItem.Queries;

public class GetAllCarritoItemsQuery : IRequest<ApiPaginationResponse<CarritoItemResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllCarritoItemsQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllCarritoItemsQuery, ApiPaginationResponse<CarritoItemResponseDto>>
{
    public async Task<ApiPaginationResponse<CarritoItemResponseDto>> Handle(GetAllCarritoItemsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.CarritoItems.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<CarritoItemResponseDto>.Ok(items.Select(x => new CarritoItemResponseDto
        {
            Id = x.Id,
            CarritoId = x.CarritoId,
            PrendaTallaId = x.PrendaTallaId,
            PrecioUnitario = x.PrecioUnitario,
            Cantidad = x.Cantidad,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
