using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class CreateCarritoItemCommand : IRequest<ApiResponse<CarritoItemResponseDto>>
{
    public required long CarritoId { get; set; }
    public required long PrendaTallaId { get; set; }
    public required decimal PrecioUnitario { get; set; }
    public required int Cantidad { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }
}

public class CreateCarritoItemCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateCarritoItemCommand, ApiResponse<CarritoItemResponseDto>>
{
    public async Task<ApiResponse<CarritoItemResponseDto>> Handle(CreateCarritoItemCommand request, CancellationToken cancellationToken)
    {
        var carrito = await _uow.Carritos.GetById(request.CarritoId);
        if (carrito is null)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        if (carrito.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail("No tienes acceso a este carrito.", ErrorType.Forbidden);
        }

        if (carrito.Estado != EstadoCarrito.ACTIVO)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail("Solo se pueden agregar ítems a un carrito con estado ACTIVO.", ErrorType.Conflict);
        }

        var prendaTalla = await _uow.PrendaTallas.GetById(request.PrendaTallaId);
        if (prendaTalla is null)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");
        }

        if (await _uow.CarritoItems.ExisteEnCarrito(request.CarritoId, request.PrendaTallaId))
        {
            return ApiResponse<CarritoItemResponseDto>.Fail("La prenda ya existe en el carrito.", ErrorType.Conflict);
        }

        var entity = new CarritoItemEntity
        {
            CarritoId = request.CarritoId,
            PrendaTallaId = request.PrendaTallaId,
            PrecioUnitario = request.PrecioUnitario,
            Cantidad = request.Cantidad,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.CarritoItems.Add(entity);
        await _uow.Complete();

        return ApiResponse<CarritoItemResponseDto>.Created(new CarritoItemResponseDto
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
