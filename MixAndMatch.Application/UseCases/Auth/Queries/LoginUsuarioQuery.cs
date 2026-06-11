using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Auth;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Auth.Queries;

public class LoginUsuarioQuery : IRequest<ApiResponse<AuthResponseDto>>
{
    public required string Email { get; set; }
    public required string Contrasenia { get; set; }
}

public class LoginUsuarioQueryHandler(IUnitOfWork _uow, IPasswordService _passwordService, IJwtService _jwtService)
    : IRequestHandler<LoginUsuarioQuery, ApiResponse<AuthResponseDto>>
{
    private const string CredencialesInvalidas = "Credenciales inválidas.";

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginUsuarioQuery request, CancellationToken cancellationToken)
    {
        var usuario = (await _uow.Repository<UsuarioEntity>().GetAll())
            .FirstOrDefault(u => u.Email == request.Email);

        if (usuario is null || string.IsNullOrEmpty(usuario.Contrasenia))
            return ApiResponse<AuthResponseDto>.Fail(CredencialesInvalidas);

        if (usuario.Activo != true)
            return ApiResponse<AuthResponseDto>.Fail("El usuario está inactivo.");

        if (!_passwordService.Verify(request.Contrasenia, usuario.Contrasenia))
            return ApiResponse<AuthResponseDto>.Fail(CredencialesInvalidas);

        var jwt = _jwtService.GenerateToken(usuario);

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token         = jwt.Token,
            ExpiraEn      = jwt.ExpiresAt,
            Id            = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Email         = usuario.Email,
            Rol           = usuario.Rol
        });
    }
}
