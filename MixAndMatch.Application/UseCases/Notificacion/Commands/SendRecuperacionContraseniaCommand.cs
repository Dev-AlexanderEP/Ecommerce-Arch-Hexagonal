using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Notificacion.Commands;

public class SendRecuperacionContraseniaCommand : IRequest<ApiResponse<bool>>
{
    public required string Email { get; set; }
}

public class SendRecuperacionContraseniaCommandHandler(
    IUnitOfWork _uow,
    ICacheService _cache,
    IEmailService _emailService,
    IEmailTemplateService _templates)
    : IRequestHandler<SendRecuperacionContraseniaCommand, ApiResponse<bool>>
{
    private static readonly TimeSpan Expiracion = TimeSpan.FromMinutes(10);

    public async Task<ApiResponse<bool>> Handle(SendRecuperacionContraseniaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByEmail(request.Email);

        if (usuario is null)
            return ApiResponse<bool>.Fail("No existe una cuenta registrada con ese correo.", ErrorType.NotFound);

        var codigo = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        await _cache.SetAsync($"otp:recovery:{request.Email}", codigo, Expiracion);

        var html = _templates.Render("RecuperacionContrasenia", new Dictionary<string, string>
        {
            ["NombreUsuario"] = usuario.NombreUsuario,
            ["Codigo"]        = codigo
        });

        await _emailService.SendAsync(request.Email, "Código de recuperación - Mix&Match", html);

        return ApiResponse<bool>.Ok(true, "Se envió el código de recuperación al correo indicado.");
    }
}
