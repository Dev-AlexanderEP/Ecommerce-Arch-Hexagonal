using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class ActualizarCantidadCarritoItemCommand : IRequest<ApiResponse<CarritoItemResponseDto>>
{
    [JsonIgnore]
    public long CarritoItemId { get; set; }

    public required int Cantidad { get; set; }

    [JsonIgnore]
    public long SolicitanteId { get; set; }
}

public class ActualizarCantidadCarritoItemCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<ActualizarCantidadCarritoItemCommand, ApiResponse<CarritoItemResponseDto>>
{
    public async Task<ApiResponse<CarritoItemResponseDto>> Handle(
        ActualizarCantidadCarritoItemCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Cantidad <= 0)
            return ApiResponse<CarritoItemResponseDto>.Fail("La cantidad debe ser mayor a 0.");

        var entity = await _uow.CarritoItems.GetById(request.CarritoItemId);
        if (entity is null)
            return ApiResponse<CarritoItemResponseDto>.Fail($"CarritoItem no encontrado para id {request.CarritoItemId}.", ErrorType.NotFound);

        var carrito = await _uow.Carritos.GetById(entity.CarritoId);
        if (carrito is null || carrito.UsuarioId != request.SolicitanteId)
            return ApiResponse<CarritoItemResponseDto>.Fail("No tienes acceso a este ítem.", ErrorType.Forbidden);

        entity.Cantidad  = request.Cantidad;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.CarritoItems.Update(entity);
        await _uow.Complete();

        return ApiResponse<CarritoItemResponseDto>.Ok(new CarritoItemResponseDto
        {
            Id             = entity.Id,
            CarritoId      = entity.CarritoId,
            PrendaTallaId  = entity.PrendaTallaId,
            PrecioUnitario = entity.PrecioUnitario,
            Cantidad       = entity.Cantidad,
            CreatedAt      = entity.CreatedAt,
            UpdatedAt      = entity.UpdatedAt
        });
    }
}
