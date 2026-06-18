using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Application.UseCases.Notificacion.Commands;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class UpdateVentaCommand : IRequest<ApiResponse<VentaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long VentaId { get; set; }
    public required string Estado { get; set; }
}

public class UpdateVentaCommandHandler(IUnitOfWork _uow, IMediator _mediator)
    : IRequestHandler<UpdateVentaCommand, ApiResponse<VentaResponseDto>>
{
    public async Task<ApiResponse<VentaResponseDto>> Handle(UpdateVentaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Ventas.GetById(request.VentaId);
        if (entity is null)
            return ApiResponse<VentaResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");

        // El formato del estado ya lo valida UpdateVentaCommandValidator (400); esto es defensa.
        if (!Enum.TryParse<EstadoVenta>(request.Estado, ignoreCase: true, out var nuevoEstado))
        {
            return ApiResponse<VentaResponseDto>.Fail($"Estado inválido: {request.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoVenta>())}.", ErrorType.Validation);
        }

        entity.Estado = nuevoEstado;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Ventas.Update(entity);
        await _uow.Complete();

        if (request.Estado == "PROCESANDO")
        {
            try
            {
                await _mediator.Send(new SendConfirmacionVentaEmailCommand { VentaId = entity.Id }, cancellationToken);
            }
            catch
            {
                // el estado ya fue guardado; el email no debe revertir la operacion
            }
        }

        return ApiResponse<VentaResponseDto>.Ok(new VentaResponseDto
        {
            Id            = entity.Id,
            UsuarioId     = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado?.ToString(),
            UpdatedAt = entity.UpdatedAt
        });
    }
}
