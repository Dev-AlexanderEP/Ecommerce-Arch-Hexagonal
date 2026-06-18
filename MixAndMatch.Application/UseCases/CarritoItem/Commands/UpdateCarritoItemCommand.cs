using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class UpdateCarritoItemCommand : IRequest<ApiResponse<CarritoItemResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long CarritoItemId { get; set; }
    public required decimal PrecioUnitario { get; set; }
    public required int Cantidad { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }
}

public class UpdateCarritoItemCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateCarritoItemCommand, ApiResponse<CarritoItemResponseDto>>
{
    public async Task<ApiResponse<CarritoItemResponseDto>> Handle(UpdateCarritoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.CarritoItems.GetById(request.CarritoItemId);
        if (entity is null)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail($"CarritoItem no encontrado para id {request.CarritoItemId}.");
        }

        var carrito = await _uow.Carritos.GetById(entity.CarritoId);
        if (carrito is null || carrito.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<CarritoItemResponseDto>.Fail("No tienes acceso a este item.", ErrorType.Forbidden);
        }

        entity.Cantidad = request.Cantidad;
        entity.PrecioUnitario = request.PrecioUnitario;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.CarritoItems.Update(entity);
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
