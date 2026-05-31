using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

namespace MixAndMatch.Application.UseCases.CarritoItem.Queries;

public class GetCarritoItemByIdQuery : IRequest<ApiResponseDto<CarritoItemResponseDto>>
{
    public required long CarritoItemId { get; set; }
}

public class GetCarritoItemByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetCarritoItemByIdQuery, ApiResponseDto<CarritoItemResponseDto>>
{
    public async Task<ApiResponseDto<CarritoItemResponseDto>> Handle(GetCarritoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<CarritoItemEntity>().GetById(request.CarritoItemId);
        if (entity is null)
        {
            return ApiResponseDto<CarritoItemResponseDto>.Fail($"CarritoItem no encontrado para id {request.CarritoItemId}.");
        }

        return ApiResponseDto<CarritoItemResponseDto>.Ok(new CarritoItemResponseDto
        {
            Id = entity.Id,
            CarritoId = entity.CarritoId,
            PrendaTallaId = entity.PrendaTallaId,
            PrecioUnitario = entity.PrecioUnitario,
            Cantidad = entity.Cantidad,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
