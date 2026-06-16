using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Auth;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Auth.Queries;

public class LoginUsuarioQuery : IRequest<ApiResponse<AuthResponseDto>>
{
    public required string Email { get; set; }
    public required string Contrasenia { get; set; }
}

public class LoginUsuarioQueryHandler(IUsuarioRepository _usuarios, IPasswordService _passwordService, IJwtService _jwtService)
    : IRequestHandler<LoginUsuarioQuery, ApiResponse<AuthResponseDto>>
{
    private const string CredencialesInvalidas = "Credenciales inválidas.";

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginUsuarioQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarios.GetByEmail(request.Email);

        if (usuario is null || string.IsNullOrEmpty(usuario.Contrasenia))
            return ApiResponse<AuthResponseDto>.Fail(CredencialesInvalidas, ErrorType.Unauthorized);

        if (usuario.Activo != true)
            return ApiResponse<AuthResponseDto>.Fail("El usuario está inactivo.", ErrorType.Forbidden);

        if (!_passwordService.Verify(request.Contrasenia, usuario.Contrasenia))
            return ApiResponse<AuthResponseDto>.Fail(CredencialesInvalidas, ErrorType.Unauthorized);

        var jwt = _jwtService.GenerateToken(usuario);

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token         = jwt.Token,
            ExpiraEn      = jwt.ExpiresAt,
            Id            = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Email         = usuario.Email,
            Rol           = usuario.Rol?.ToString()
        });
    }
}
