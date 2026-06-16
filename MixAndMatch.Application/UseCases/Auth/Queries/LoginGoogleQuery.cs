using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Auth;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Auth.Queries;

public class LoginGoogleQuery : IRequest<ApiResponse<AuthResponseDto>>
{
    public required string IdToken { get; set; }
}

public class LoginGoogleQueryHandler(IUnitOfWork _uow, IGoogleAuthService _googleAuth, IJwtService _jwtService)
    : IRequestHandler<LoginGoogleQuery, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginGoogleQuery request, CancellationToken cancellationToken)
    {
        var info = await _googleAuth.ValidateIdToken(request.IdToken);
        if (info is null)
            return ApiResponse<AuthResponseDto>.Fail("Token de Google inválido.");

        var repo     = _uow.Repository<UsuarioEntity>();
        var usuarios = await repo.GetAll();

        var usuario = usuarios.FirstOrDefault(u => u.Email == info.Email);

        if (usuario is null)
        {
            usuario = new UsuarioEntity
            {
                NombreUsuario = GenerarNombreUsuario(info.Email, usuarios),
                Email         = info.Email,
                Contrasenia   = null,            // cuenta Google: sin credencial local
                Rol           = RolUsuario.CLIENTE,
                Activo        = true,
                CreatedAt     = DateTime.UtcNow
            };

            await repo.Add(usuario);
            await _uow.Complete();
        }
        else if (usuario.Activo != true)
        {
            return ApiResponse<AuthResponseDto>.Fail("El usuario está inactivo.");
        }

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

    // nombre_usuario es UNIQUE: parte local del email + sufijo numérico si está tomado
    private static string GenerarNombreUsuario(string email, IEnumerable<UsuarioEntity> usuarios)
    {
        var baseName  = email.Split('@')[0];
        var candidato = baseName;
        var sufijo    = 1;

        while (usuarios.Any(u => u.NombreUsuario == candidato))
            candidato = $"{baseName}{sufijo++}";

        return candidato;
    }
}
