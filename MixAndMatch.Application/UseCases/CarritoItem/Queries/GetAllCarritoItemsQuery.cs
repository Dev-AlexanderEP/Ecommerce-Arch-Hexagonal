using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

namespace MixAndMatch.Application.UseCases.CarritoItem.Queries;

public class GetAllCarritoItemsQuery : IRequest<ApiResponseDto<IEnumerable<CarritoItemResponseDto>>>
{
}

public class GetAllCarritoItemsQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllCarritoItemsQuery, ApiResponseDto<IEnumerable<CarritoItemResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<CarritoItemResponseDto>>> Handle(GetAllCarritoItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<CarritoItemEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<CarritoItemResponseDto>>.Fail("No se encontraron ítems de carrito.");
        }

        return ApiResponseDto<IEnumerable<CarritoItemResponseDto>>.Ok(items.Select(x => new CarritoItemResponseDto
        {
            Id = x.Id,
            CarritoId = x.CarritoId,
            PrendaTallaId = x.PrendaTallaId,
            PrecioUnitario = x.PrecioUnitario,
            Cantidad = x.Cantidad,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
