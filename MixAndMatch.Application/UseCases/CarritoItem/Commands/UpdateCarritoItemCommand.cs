using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class UpdateCarritoItemCommand : IRequest<ApiResponse<CarritoItemResponseDto>>
{
    public required long CarritoItemId { get; set; }
    public required decimal PrecioUnitario { get; set; }
    public required int Cantidad { get; set; }
}

public class UpdateCarritoItemCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateCarritoItemCommand, ApiResponse<CarritoItemResponseDto>>
{
    public async Task<ApiResponse<CarritoItemResponseDto>> Handle(UpdateCarritoItemCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CarritoItemEntity>();
        var entity = await repo.GetById(request.CarritoItemId);
        if (entity is null)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail($"CarritoItem no encontrado para id {request.CarritoItemId}.");
        }

        if (request.Cantidad <= 0)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail("La cantidad debe ser mayor a cero.");
        }

        if (request.PrecioUnitario <= 0)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail("El precio unitario debe ser mayor a cero.");
        }

        entity.Cantidad = request.Cantidad;
        entity.PrecioUnitario = request.PrecioUnitario;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<CarritoItemResponseDto>.Ok(new CarritoItemResponseDto
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
