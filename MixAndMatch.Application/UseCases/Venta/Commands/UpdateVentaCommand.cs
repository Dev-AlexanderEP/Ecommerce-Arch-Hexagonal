using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Application.UseCases.Notificacion.Commands;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class UpdateVentaCommand : IRequest<ApiResponse<VentaResponseDto>>
{
    public required long VentaId { get; set; }
    public required string Estado { get; set; }
}

public class UpdateVentaCommandHandler(IUnitOfWork _uow, IMediator _mediator)
    : IRequestHandler<UpdateVentaCommand, ApiResponse<VentaResponseDto>>
{
    private static readonly HashSet<string> EstadosValidos = ["PENDIENTE", "PROCESANDO", "ENVIADO", "ENTREGADO", "CANCELADO"];

    public async Task<ApiResponse<VentaResponseDto>> Handle(UpdateVentaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<VentaEntity>();
        var entity = await repo.GetById(request.VentaId);
        if (entity is null)
            return ApiResponse<VentaResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");

        var estado = (request.Estado ?? string.Empty).Trim().ToUpperInvariant();
        if (!EstadosValidos.Contains(estado))
            return ApiResponse<VentaResponseDto>.Fail($"Estado invalido. Valores permitidos: {string.Join(", ", EstadosValidos)}.");

        entity.Estado    = estado;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        if (estado == "PROCESANDO")
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
            Estado        = entity.Estado,
            UpdatedAt     = entity.UpdatedAt
        });
    }
}
