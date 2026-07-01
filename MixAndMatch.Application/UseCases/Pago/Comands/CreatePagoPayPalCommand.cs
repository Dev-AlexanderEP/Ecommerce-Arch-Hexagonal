using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class CreatePagoPayPalCommand : IRequest<ApiResponse<PagoPayPalIniciadoDto>>
{
    public required long VentaId { get; set; }
    public required long MetodoId { get; set; }

    [JsonIgnore] public long SolicitanteId { get; set; }
}

public class CreatePagoPayPalCommandHandler(IUnitOfWork _uow, IPayPalGatewayService _gateway)
    : IRequestHandler<CreatePagoPayPalCommand, ApiResponse<PagoPayPalIniciadoDto>>
{
    public async Task<ApiResponse<PagoPayPalIniciadoDto>> Handle(CreatePagoPayPalCommand request, CancellationToken cancellationToken)
    {
        var venta = await _uow.Ventas.GetByIdConDetalles(request.VentaId);
        if (venta is null)
        {
            return ApiResponse<PagoPayPalIniciadoDto>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.Validation);
        }

        if (venta.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<PagoPayPalIniciadoDto>.Fail("No tienes acceso a esta venta.", ErrorType.Forbidden);
        }

        var metodo = await _uow.MetodoPagos.GetById(request.MetodoId);
        if (metodo is null)
        {
            return ApiResponse<PagoPayPalIniciadoDto>.Fail($"Método de pago no encontrado para id {request.MetodoId}.", ErrorType.Validation);
        }

        var monto = venta.Total;
        if (monto <= 0m)
        {
            return ApiResponse<PagoPayPalIniciadoDto>.Fail("La venta no tiene detalles; no hay monto a pagar.", ErrorType.Validation);
        }

        var entity = new PagoEntity
        {
            VentaId = request.VentaId,
            MetodoId = request.MetodoId,
            Monto = monto,
            Estado = EstadoPago.PENDIENTE,
            FechaCreacion = DateTime.UtcNow
        };

        await _uow.Repository<PagoEntity>().Add(entity);
        await _uow.Complete();

        try
        {
            var orden = await _gateway.CrearOrden(monto, $"pago-{entity.Id}");

            return ApiResponse<PagoPayPalIniciadoDto>.Created(new PagoPayPalIniciadoDto
            {
                PagoId = entity.Id,
                OrderId = orden.OrderId
            });
        }
        catch (Exception)
        {
            entity.Estado = EstadoPago.FALLIDO;
            entity.UpdatedAt = DateTime.UtcNow;
            await _uow.Repository<PagoEntity>().Update(entity);
            await _uow.Complete();

            return ApiResponse<PagoPayPalIniciadoDto>.Fail("No se pudo iniciar el pago con PayPal.", ErrorType.Validation);
        }
    }
}
