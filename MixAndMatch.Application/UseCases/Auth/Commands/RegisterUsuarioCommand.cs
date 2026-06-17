using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Auth;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Auth.Commands;

public class RegisterUsuarioCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public required string NombreUsuario { get; set; }
    public required string Email { get; set; }
    public required string Contrasenia { get; set; }
}

public class RegisterUsuarioCommandHandler(IUnitOfWork _uow, IPasswordService _passwordService, IJwtService _jwtService)
    : IRequestHandler<RegisterUsuarioCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Usuarios.ExistsByEmail(request.Email))
            return ApiResponse<AuthResponseDto>.Fail($"Ya existe un usuario con el email {request.Email}.", ErrorType.Conflict);

        if (await _uow.Usuarios.ExistsByNombreUsuario(request.NombreUsuario))
            return ApiResponse<AuthResponseDto>.Fail($"El nombre de usuario {request.NombreUsuario} ya está en uso.", ErrorType.Conflict);

        var entity = new UsuarioEntity
        {
            NombreUsuario = request.NombreUsuario,
            Email         = request.Email,
            Contrasenia   = _passwordService.Hash(request.Contrasenia),
            Rol           = RolUsuario.CLIENTE,
            Activo        = true,
            CreatedAt     = DateTime.UtcNow
        };

        await _uow.Usuarios.Add(entity);
        await _uow.Complete();

        var jwt = _jwtService.GenerateToken(entity);

        return ApiResponse<AuthResponseDto>.Created(new AuthResponseDto
        {
            Token         = jwt.Token,
            ExpiraEn      = jwt.ExpiresAt,
            Id            = entity.Id,
            NombreUsuario = entity.NombreUsuario,
            Email         = entity.Email,
            Rol           = entity.Rol?.ToString()
        });
    }
}
