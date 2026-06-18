using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Notificacion.Commands;

public class SendConfirmacionVentaEmailCommand : IRequest<ApiResponse<bool>>
{
    public required long VentaId { get; set; }
}

public class SendConfirmacionVentaEmailCommandHandler(
    IUnitOfWork _uow,
    IEmailService _emailService,
    IEmailTemplateService _templates)
    : IRequestHandler<SendConfirmacionVentaEmailCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(SendConfirmacionVentaEmailCommand request, CancellationToken cancellationToken)
    {
        var venta = await _uow.Ventas.GetByIdConDetalles(request.VentaId);
        if (venta is null)
            return ApiResponse<bool>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.NotFound);

        var usuario = await _uow.Usuarios.GetById(venta.UsuarioId);
        if (usuario is null)
            return ApiResponse<bool>.Fail("Usuario de la venta no encontrado.", ErrorType.NotFound);

        var html = _templates.Render("ConfirmacionVenta", new Dictionary<string, string>
        {
            ["VentaId"] = request.VentaId.ToString(),
            ["Total"]   = venta.Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
        });

        await _emailService.SendAsync(usuario.Email, $"Confirmación de pedido #{request.VentaId} - Mix&Match", html);

        return ApiResponse<bool>.Ok(true, "Correo de confirmación de venta enviado.");
    }
}
