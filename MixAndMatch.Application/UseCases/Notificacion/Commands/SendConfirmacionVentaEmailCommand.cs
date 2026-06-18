using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;
using VentaDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

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
        var venta = await _uow.Repository<VentaEntity>().GetById(request.VentaId);
        if (venta is null)
            return ApiResponse<bool>.Fail($"Venta no encontrada para id {request.VentaId}.");

        var usuario = await _uow.Repository<UsuarioEntity>().GetById(venta.UsuarioId);
        if (usuario is null)
            return ApiResponse<bool>.Fail("Usuario de la venta no encontrado.");

        var detalles = (await _uow.Repository<VentaDetalleEntity>().GetAll())
            .Where(d => d.VentaId == request.VentaId);

        var total = detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

        var html = _templates.Render("ConfirmacionVenta", new Dictionary<string, string>
        {
            ["VentaId"] = request.VentaId.ToString(),
            ["Total"]   = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
        });

        await _emailService.SendAsync(usuario.Email, $"Confirmacion de pedido #{request.VentaId} - Mix&Match", html);

        return ApiResponse<bool>.Ok(true, "Correo de confirmacion de venta enviado.");
    }
}
