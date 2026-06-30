using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class CreatePagoCommand : IRequest<ApiResponse<PagoResponseDto>>
{
    public long VentaId { get; set; }
    public long MetodoId { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }
}

public class CreatePagoCommandHandler(IUnitOfWork _uow, IMediator _mediator)
    : IRequestHandler<CreatePagoCommand, ApiResponse<PagoResponseDto>>
{
    private const string MetodoAutoConfirmable = "TARJETA_CREDITO";

    public async Task<ApiResponse<PagoResponseDto>> Handle(CreatePagoCommand request, CancellationToken cancellationToken)
    {
        // Se carga con detalles porque el monto se calcula desde venta.Total.
        var venta = await _uow.Ventas.GetByIdConDetalles(request.VentaId);
        if (venta is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.Validation);
        }

        // Solo el dueño de la venta puede pagarla.
        if (venta.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<PagoResponseDto>.Fail("No tienes acceso a esta venta.", ErrorType.Forbidden);
        }

        var metodo = await _uow.MetodoPagos.GetById(request.MetodoId);
        if (metodo is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Método de pago no encontrado para id {request.MetodoId}.", ErrorType.Validation);
        }

        // El monto NO se confía al body: se calcula desde el total de la venta.
        var monto = venta.Total;
        if (monto <= 0m)
        {
            return ApiResponse<PagoResponseDto>.Fail("La venta no tiene detalles; no hay monto a pagar.", ErrorType.Validation);
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

        // La tarjeta de crédito es simbólica (no hay pasarela real): se autoconfirma de inmediato.
        // PayPal/Yape/MercadoPago quedan PENDIENTE hasta que su webhook confirme el pago.
        if (string.Equals(metodo.TipoPago, MetodoAutoConfirmable, StringComparison.OrdinalIgnoreCase))
        {
            var confirmado = await _mediator.Send(new ConfirmarPagoCommand { PagoId = entity.Id }, cancellationToken);
            if (confirmado.Success)
            {
                return confirmado;
            }
        }

        return ApiResponse<PagoResponseDto>.Created(new PagoResponseDto
        {
            Id = entity.Id,
            VentaId = entity.VentaId,
            MetodoId = entity.MetodoId,
            Monto = entity.Monto,
            Estado = entity.Estado.ToString(),
            FechaCreacion = entity.FechaCreacion,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
