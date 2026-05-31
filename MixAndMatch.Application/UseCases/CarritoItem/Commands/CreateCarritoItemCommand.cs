using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class CreateCarritoItemCommand : IRequest<ApiResponseDto<CarritoItemResponseDto>>
{
    public required long CarritoId { get; set; }
    public required long PrendaTallaId { get; set; }
    public required decimal PrecioUnitario { get; set; }
    public required int Cantidad { get; set; }
}

public class CreateCarritoItemCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateCarritoItemCommand, ApiResponseDto<CarritoItemResponseDto>>
{
    public async Task<ApiResponseDto<CarritoItemResponseDto>> Handle(CreateCarritoItemCommand request, CancellationToken cancellationToken)
    {
        var carrito = await _uow.Repository<CarritoEntity>().GetById(request.CarritoId);
        if (carrito is null)
        {
            return ApiResponseDto<CarritoItemResponseDto>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        if (carrito.Estado != "ACTIVO")
        {
            return ApiResponseDto<CarritoItemResponseDto>.Fail("Solo se pueden agregar ítems a un carrito con estado ACTIVO.");
        }

        var prendaTalla = await _uow.Repository<PrendaTallaEntity>().GetById(request.PrendaTallaId);
        if (prendaTalla is null)
        {
            return ApiResponseDto<CarritoItemResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");
        }

        var items = await _uow.Repository<CarritoItemEntity>().GetAll();
        if (items.Any(x => x.CarritoId == request.CarritoId && x.PrendaTallaId == request.PrendaTallaId))
        {
            return ApiResponseDto<CarritoItemResponseDto>.Fail("La prenda ya existe en el carrito.");
        }

        if (request.Cantidad <= 0)
        {
            return ApiResponseDto<CarritoItemResponseDto>.Fail("La cantidad debe ser mayor a cero.");
        }

        if (request.PrecioUnitario <= 0)
        {
            return ApiResponseDto<CarritoItemResponseDto>.Fail("El precio unitario debe ser mayor a cero.");
        }

        var entity = new CarritoItemEntity
        {
            CarritoId = request.CarritoId,
            PrendaTallaId = request.PrendaTallaId,
            PrecioUnitario = request.PrecioUnitario,
            Cantidad = request.Cantidad,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<CarritoItemEntity>().Add(entity);
        await _uow.Complete();

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
