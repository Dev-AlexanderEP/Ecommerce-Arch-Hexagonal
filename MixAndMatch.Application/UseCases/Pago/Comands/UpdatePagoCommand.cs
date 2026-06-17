using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class UpdatePagoCommand : IRequest<ApiResponse<PagoResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long Id { get; set; }
    public long VentaId { get; set; }
    public long MetodoId { get; set; }
    public decimal Monto { get; set; }
    public required string Estado { get; set; }
}

public class UpdatePagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdatePagoCommand, ApiResponse<PagoResponseDto>>
{
    public async Task<ApiResponse<PagoResponseDto>> Handle(UpdatePagoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<PagoEntity>().GetById(request.Id);
        if (entity is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Pago no encontrado para id {request.Id}.");
        }

        var venta = await _uow.Ventas.GetById(request.VentaId);
        if (venta is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.Validation);
        }

        var metodo = await _uow.MetodoPagos.GetById(request.MetodoId);
        if (metodo is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Método de pago no encontrado para id {request.MetodoId}.", ErrorType.Validation);
        }

        // El formato del estado ya lo valida UpdatePagoCommandValidator (400); esto es defensa.
        if (!Enum.TryParse<EstadoPago>(request.Estado, ignoreCase: true, out var estadoPago))
        {
            return ApiResponse<PagoResponseDto>.Fail($"Estado inválido: {request.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoPago>())}.", ErrorType.Validation);
        }

        entity.VentaId = request.VentaId;
        entity.MetodoId = request.MetodoId;
        entity.Monto = request.Monto;
        entity.Estado = estadoPago;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Repository<PagoEntity>().Update(entity);
        await _uow.Complete();

        return ApiResponse<PagoResponseDto>.Ok(new PagoResponseDto
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
