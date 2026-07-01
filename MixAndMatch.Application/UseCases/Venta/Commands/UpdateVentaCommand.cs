using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class UpdateVentaCommand : IRequest<ApiResponse<VentaResponseDto>>
{
    [JsonIgnore] public long VentaId { get; set; }
    [JsonIgnore] public long SolicitanteId { get; set; }
    [JsonIgnore] public bool EsAdmin { get; set; }
    public required string Estado { get; set; }
}

public class UpdateVentaCommandHandler(IUnitOfWork _uow, IEmailService _emailService, IEmailTemplateService _templates)
    : IRequestHandler<UpdateVentaCommand, ApiResponse<VentaResponseDto>>
{
    public async Task<ApiResponse<VentaResponseDto>> Handle(UpdateVentaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Ventas.GetById(request.VentaId);
        if (entity is null)
            return ApiResponse<VentaResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");

        if (!request.EsAdmin && entity.UsuarioId != request.SolicitanteId)
            return ApiResponse<VentaResponseDto>.Fail("No tienes acceso a esta venta.", ErrorType.Forbidden);

        // El formato del estado ya lo valida UpdateVentaCommandValidator (400); esto es defensa.
        if (!Enum.TryParse<EstadoVenta>(request.Estado, ignoreCase: true, out var nuevoEstado))
            return ApiResponse<VentaResponseDto>.Fail($"Estado inválido: {request.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoVenta>())}.", ErrorType.Validation);

        if (!request.EsAdmin && nuevoEstado != EstadoVenta.CANCELADO)
            return ApiResponse<VentaResponseDto>.Fail("Solo puedes cancelar tu propia venta.", ErrorType.Forbidden);

        entity.Estado = nuevoEstado;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Ventas.Update(entity);
        await _uow.Complete();

        if (nuevoEstado == EstadoVenta.PAGADO)
        {
            try
            {
                var ventaConDetalles = await _uow.Ventas.GetByIdConDetalles(entity.Id);
                var usuario = ventaConDetalles is not null ? await _uow.Usuarios.GetById(entity.UsuarioId) : null;
                if (ventaConDetalles is not null && usuario is not null)
                {
                    var html = _templates.Render("ConfirmacionVenta", new Dictionary<string, string>
                    {
                        ["VentaId"] = entity.Id.ToString(),
                        ["Total"]   = ventaConDetalles.Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                    });
                    await _emailService.SendAsync(usuario.Email, $"Confirmación de pedido #{entity.Id} - Mix&Match", html);
                }
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
            Estado        = entity.Estado?.ToString(),
            UpdatedAt     = entity.UpdatedAt
        });
    }
}
