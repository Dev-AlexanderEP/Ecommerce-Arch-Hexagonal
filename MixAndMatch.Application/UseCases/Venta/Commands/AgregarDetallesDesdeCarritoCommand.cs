using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class AgregarDetallesDesdeCarritoCommand : IRequest<ApiResponse<List<VentasDetalleResponseDto>>>
{
    public required long VentaId { get; set; }
    public required long CarritoId { get; set; }

    [JsonIgnore]
    public long SolicitanteId { get; set; }
}

public class AgregarDetallesDesdeCarritoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<AgregarDetallesDesdeCarritoCommand, ApiResponse<List<VentasDetalleResponseDto>>>
{
    public async Task<ApiResponse<List<VentasDetalleResponseDto>>> Handle(
        AgregarDetallesDesdeCarritoCommand request,
        CancellationToken cancellationToken)
    {
        var venta = await _uow.Ventas.GetById(request.VentaId);
        if (venta is null)
            return ApiResponse<List<VentasDetalleResponseDto>>.Fail(
                $"Venta no encontrada para id {request.VentaId}.", ErrorType.NotFound);

        if (venta.UsuarioId != request.SolicitanteId)
            return ApiResponse<List<VentasDetalleResponseDto>>.Fail(
                "No tienes acceso a esta venta.", ErrorType.Forbidden);

        var carrito = await _uow.Carritos.GetById(request.CarritoId);
        if (carrito is null)
            return ApiResponse<List<VentasDetalleResponseDto>>.Fail(
                $"Carrito no encontrado para id {request.CarritoId}.", ErrorType.NotFound);

        if (carrito.UsuarioId != request.SolicitanteId)
            return ApiResponse<List<VentasDetalleResponseDto>>.Fail(
                "No tienes acceso a este carrito.", ErrorType.Forbidden);

        var items = await _uow.CarritoItems.GetByCarritoId(request.CarritoId);
        if (items.Count == 0)
            return ApiResponse<List<VentasDetalleResponseDto>>.Fail(
                "El carrito no tiene ítems.", ErrorType.Validation);

        var detalles = items.Select(i => new VentasDetalleEntity
        {
            VentaId        = request.VentaId,
            PrendaTallaId  = i.PrendaTallaId,
            Cantidad       = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario,
            CreatedAt      = DateTime.UtcNow
        }).ToList();

        foreach (var detalle in detalles)
            await _uow.VentasDetalles.Add(detalle);

        await _uow.Complete();

        return ApiResponse<List<VentasDetalleResponseDto>>.Created(
            detalles.Select(d => new VentasDetalleResponseDto
            {
                Id             = d.Id,
                VentaId        = d.VentaId,
                PrendaTallaId  = d.PrendaTallaId,
                Cantidad       = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                CreatedAt      = d.CreatedAt,
                UpdatedAt      = d.UpdatedAt
            }).ToList());
    }
}
