using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

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
        var usuario = (await _uow.Repository<UsuarioEntity>().GetAll())
            .FirstOrDefault(u => u.Email == request.Email);

        if (usuario is null)
            return ApiResponse<bool>.Fail("No existe una cuenta registrada con ese correo.");

        var codigo = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        await _cache.SetAsync($"otp:recovery:{request.Email}", codigo, Expiracion);

        var html = _templates.Render("RecuperacionContrasenia", new Dictionary<string, string>
        {
            ["NombreUsuario"] = usuario.NombreUsuario,
            ["Codigo"]        = codigo
        });

        await _emailService.SendAsync(request.Email, "Codigo de recuperacion - Mix&Match", html);

        return ApiResponse<bool>.Ok(true, "Se envio el codigo de recuperacion al correo indicado.");
    }
}
