using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

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
        var (items, total) = await _uow.Repository<CarritoItemEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<CarritoItemResponseDto>.Fail("No se encontraron ítems de carrito.");
        }

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
