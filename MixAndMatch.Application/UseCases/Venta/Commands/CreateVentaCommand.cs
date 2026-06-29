using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class CreateVentaCommand : IRequest<ApiResponse<VentaResponseDto>>
{
    [JsonIgnore]
    public long SolicitanteId { get; set; }
}

public class CreateVentaCommandHandler(
    IUnitOfWork _uow,
    IEmailService _email,
    IEmailTemplateService _templates)
    : IRequestHandler<CreateVentaCommand, ApiResponse<VentaResponseDto>>
{
    public async Task<ApiResponse<VentaResponseDto>> Handle(CreateVentaCommand request, CancellationToken cancellationToken)
    {
        var entity = new VentaEntity
        {
            UsuarioId = request.SolicitanteId,
            Estado = EstadoVenta.PENDIENTE,
            FechaCreacion = DateTime.UtcNow
        };

        await _uow.Ventas.Add(entity);
        await _uow.Complete();

        _ = EnviarConfirmacionAsync(entity.Id, request.SolicitanteId);

        return ApiResponse<VentaResponseDto>.Created(new VentaResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado?.ToString(),
            UpdatedAt = entity.UpdatedAt
        });
    }

    private async Task EnviarConfirmacionAsync(long ventaId, long usuarioId)
    {
        try
        {
            var usuario = await _uow.Usuarios.GetById(usuarioId);
            if (usuario is null) return;

            var total = await _uow.Carritos.GetTotalCarritoActivo(usuarioId);

            var html = _templates.Render("ConfirmacionVenta", new Dictionary<string, string>
            {
                ["VentaId"] = ventaId.ToString(),
                ["Total"]   = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
            });

            await _email.SendAsync(usuario.Email, $"Confirmación de pedido #{ventaId} - Mix&Match", html);
        }
        catch
        {
            // El email no debe bloquear ni revertir la creación de la venta.
        }
    }
}
