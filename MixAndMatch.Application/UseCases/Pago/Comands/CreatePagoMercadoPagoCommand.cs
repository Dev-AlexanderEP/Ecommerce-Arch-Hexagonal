using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Application.Services;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class CreatePagoMercadoPagoCommand : IRequest<ApiResponse<PagoResponseDto>>
{
    public required long VentaId { get; set; }
    public required long MetodoId { get; set; }
    public required string Token { get; set; }
    public required string PaymentMethodId { get; set; }
    public string? IssuerId { get; set; }
    public required int Installments { get; set; }
    public required string PayerEmail { get; set; }
    public string? IdentificacionTipo { get; set; }
    public string? IdentificacionNumero { get; set; }

    [JsonIgnore] public long SolicitanteId { get; set; }
}

public class CreatePagoMercadoPagoCommandHandler(IUnitOfWork _uow, IPagoGatewayService _gateway, IConfirmacionPagoService _confirmacion)
    : IRequestHandler<CreatePagoMercadoPagoCommand, ApiResponse<PagoResponseDto>>
{
    public async Task<ApiResponse<PagoResponseDto>> Handle(CreatePagoMercadoPagoCommand request, CancellationToken cancellationToken)
    {
        var venta = await _uow.Ventas.GetByIdConDetalles(request.VentaId);
        if (venta is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.Validation);
        }

        if (venta.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<PagoResponseDto>.Fail("No tienes acceso a esta venta.", ErrorType.Forbidden);
        }

        var metodo = await _uow.MetodoPagos.GetById(request.MetodoId);
        if (metodo is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Método de pago no encontrado para id {request.MetodoId}.", ErrorType.Validation);
        }

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

        PagoGatewayResultado resultado;
        try
        {
            resultado = await _gateway.CrearPago(new CrearPagoGatewayRequest(
                Monto: monto,
                Token: request.Token,
                PaymentMethodId: request.PaymentMethodId,
                IssuerId: request.IssuerId,
                Installments: request.Installments,
                PayerEmail: request.PayerEmail,
                IdentificacionTipo: request.IdentificacionTipo,
                IdentificacionNumero: request.IdentificacionNumero,
                ExternalReference: $"pago-{entity.Id}"));
        }
        catch (Exception)
        {
            return ApiResponse<PagoResponseDto>.Fail("No se pudo procesar el pago con Mercado Pago.", ErrorType.Validation);
        }

        return resultado.Status switch
        {
            "approved" => await _confirmacion.ConfirmarAsync(entity.Id, cancellationToken),

            "rejected" or "cancelled" => await MarcarFallido(entity, resultado.StatusDetail),

            // "in_process" / "pending": queda PENDIENTE, se confirma luego por webhook.
            _ => ApiResponse<PagoResponseDto>.Created(new PagoResponseDto
            {
                Id = entity.Id,
                VentaId = entity.VentaId,
                MetodoId = entity.MetodoId,
                Monto = entity.Monto,
                Estado = entity.Estado.ToString(),
                FechaCreacion = entity.FechaCreacion,
                UpdatedAt = entity.UpdatedAt
            })
        };
    }

    private async Task<ApiResponse<PagoResponseDto>> MarcarFallido(PagoEntity entity, string? statusDetail)
    {
        entity.Estado = EstadoPago.FALLIDO;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.Repository<PagoEntity>().Update(entity);
        await _uow.Complete();

        return ApiResponse<PagoResponseDto>.Fail($"Pago rechazado: {statusDetail}", ErrorType.Validation);
    }
}
