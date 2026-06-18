using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.CarritoItem.Queries;

public class GetCarritoItemByIdQuery : IRequest<ApiResponse<CarritoItemResponseDto>>
{
    public required long CarritoItemId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class GetCarritoItemByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetCarritoItemByIdQuery, ApiResponse<CarritoItemResponseDto>>
{
    public async Task<ApiResponse<CarritoItemResponseDto>> Handle(GetCarritoItemByIdQuery request, CancellationToken cancellationToken)
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
