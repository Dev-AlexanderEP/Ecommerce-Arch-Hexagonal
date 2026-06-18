using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Notificacion.Commands;

public class SendWelcomeEmailCommand : IRequest<ApiResponse<bool>>
{
    public required string Email { get; set; }
    public required string NombreUsuario { get; set; }
}

public class SendWelcomeEmailCommandHandler(IEmailService _emailService, IEmailTemplateService _templates)
    : IRequestHandler<SendWelcomeEmailCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(SendWelcomeEmailCommand request, CancellationToken cancellationToken)
    {
        var html = _templates.Render("Welcome", new Dictionary<string, string>
        {
            ["NombreUsuario"] = request.NombreUsuario,
            ["UrlTienda"]     = "#" // TODO: reemplazar con la URL del frontend cuando exista
        });

        await _emailService.SendAsync(request.Email, "Bienvenido a Mix&Match", html);

        return ApiResponse<bool>.Ok(true, "Correo de bienvenida enviado.");
    }
}
