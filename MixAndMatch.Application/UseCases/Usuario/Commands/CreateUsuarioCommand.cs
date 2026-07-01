using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Usuario.Commands;

public class CreateUsuarioCommand : IRequest<ApiResponse<UsuarioResponseDto>>
{
    public required string NombreUsuario { get; set; }
    public required string Email { get; set; }
    public required string Contrasenia { get; set; }
    public string? Rol { get; set; }
    public bool Activo { get; set; } = true;
}

public class CreateUsuarioCommandHandler(IUnitOfWork _uow, IPasswordService _passwordService, IEmailService _emailService, IEmailTemplateService _templates)
    : IRequestHandler<CreateUsuarioCommand, ApiResponse<UsuarioResponseDto>>
{
    public async Task<ApiResponse<UsuarioResponseDto>> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var nombreUsuario = request.NombreUsuario.Trim();
        var email = request.Email.Trim();

        if (await _uow.Usuarios.ExistsByEmail(email))
        {
            return ApiResponse<UsuarioResponseDto>.Fail($"Ya existe un usuario con el email {email}.", ErrorType.Conflict);
        }

        if (await _uow.Usuarios.ExistsByNombreUsuario(nombreUsuario))
        {
            return ApiResponse<UsuarioResponseDto>.Fail($"Ya existe un usuario con el nombre {nombreUsuario}.", ErrorType.Conflict);
        }

        // El formato del rol ya lo valida el validador (400); por defecto CLIENTE.
        var rol = RolUsuario.CLIENTE;
        if (!string.IsNullOrWhiteSpace(request.Rol))
        {
            Enum.TryParse(request.Rol, ignoreCase: true, out rol);
        }

        var entity = new UsuarioEntity
        {
            NombreUsuario = nombreUsuario,
            Email         = email,
            Contrasenia   = _passwordService.Hash(request.Contrasenia),
            Rol           = rol,
            Activo        = request.Activo,
            CreatedAt     = DateTime.UtcNow
        };

        await _uow.Usuarios.Add(entity);
        await _uow.Complete();

        try
        {
            var html = _templates.Render("Welcome", new Dictionary<string, string>
            {
                ["NombreUsuario"] = entity.NombreUsuario,
                ["UrlTienda"]     = "#"
            });
            await _emailService.SendAsync(entity.Email, "Bienvenido a Mix&Match", html);
        }
        catch
        {
            // el registro no debe fallar si el SMTP no esta disponible o configurado
        }

        return ApiResponse<UsuarioResponseDto>.Created(new UsuarioResponseDto
        {
            Id            = entity.Id,
            NombreUsuario = entity.NombreUsuario,
            Email         = entity.Email,
            Rol           = entity.Rol?.ToString(),
            Activo        = entity.Activo,
            CreatedAt     = entity.CreatedAt,
            UpdatedAt     = entity.UpdatedAt
        });
    }
}
