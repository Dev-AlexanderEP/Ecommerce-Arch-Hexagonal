using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class AgregarOIncrementarCarritoItemCommand : IRequest<ApiResponse<CarritoItemResponseDto>>
{
    public required long CarritoId { get; set; }
    public required long PrendaId { get; set; }
    public required long TallaId { get; set; }

    [JsonIgnore]
    public long SolicitanteId { get; set; }
}

public class AgregarOIncrementarCarritoItemCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<AgregarOIncrementarCarritoItemCommand, ApiResponse<CarritoItemResponseDto>>
{
    public async Task<ApiResponse<CarritoItemResponseDto>> Handle(
        AgregarOIncrementarCarritoItemCommand request,
        CancellationToken cancellationToken)
    {
        var carrito = await _uow.Carritos.GetById(request.CarritoId);
        if (carrito is null)
            return ApiResponse<CarritoItemResponseDto>.Fail($"Carrito no encontrado para id {request.CarritoId}.", ErrorType.NotFound);

        if (carrito.UsuarioId != request.SolicitanteId)
            return ApiResponse<CarritoItemResponseDto>.Fail("No tienes acceso a este carrito.", ErrorType.Forbidden);

        if (carrito.Estado != EstadoCarrito.ACTIVO)
            return ApiResponse<CarritoItemResponseDto>.Fail("Solo se pueden agregar ítems a un carrito con estado ACTIVO.", ErrorType.Conflict);

        var prendaTalla = await _uow.PrendaTallas.BuscarPorPrendaYTalla(request.PrendaId, request.TallaId);
        if (prendaTalla is null)
            return ApiResponse<CarritoItemResponseDto>.Fail("No se encontró la combinación prenda-talla.", ErrorType.NotFound);

        if (prendaTalla.Stock <= 0)
            return ApiResponse<CarritoItemResponseDto>.Fail("No hay stock disponible para esta prenda-talla.", ErrorType.Conflict);

        var existente = await _uow.CarritoItems.BuscarPorCarritoPrendaTalla(request.CarritoId, prendaTalla.Id);

        CarritoItemEntity entity;

        if (existente is not null)
        {
            existente.Cantidad++;
            existente.UpdatedAt = DateTime.UtcNow;
            await _uow.CarritoItems.Update(existente);
            entity = existente;
        }
        else
        {
            var prenda = await _uow.Prendas.GetById(prendaTalla.PrendaId);
            if (prenda is null)
                return ApiResponse<CarritoItemResponseDto>.Fail("La prenda asociada no fue encontrada.", ErrorType.NotFound);

            entity = new CarritoItemEntity
            {
                CarritoId      = request.CarritoId,
                PrendaTallaId  = prendaTalla.Id,
                PrecioUnitario = prenda.Precio,
                Cantidad       = 1,
                CreatedAt      = DateTime.UtcNow
            };
            await _uow.CarritoItems.Add(entity);
        }

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
