using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Usuario.Commands;

public class UpdateUsuarioCommand : IRequest<ApiResponse<UsuarioResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long UsuarioId { get; set; }
    public required string NombreUsuario { get; set; }
    public required string Email { get; set; }
    public string? Rol { get; set; }
    public required bool Activo { get; set; }
    public string? NuevaContrasenia { get; set; }
}

public class UpdateUsuarioCommandHandler(IUnitOfWork _uow, IPasswordService _passwordService)
    : IRequestHandler<UpdateUsuarioCommand, ApiResponse<UsuarioResponseDto>>
{
    public async Task<ApiResponse<UsuarioResponseDto>> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Usuarios.GetById(request.UsuarioId);
        if (entity is null)
        {
            return ApiResponse<UsuarioResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        var nombreUsuario = request.NombreUsuario.Trim();
        var email = request.Email.Trim();

        if (await _uow.Usuarios.ExistsByEmail(email, request.UsuarioId))
        {
            return ApiResponse<UsuarioResponseDto>.Fail($"El email {email} ya esta en uso por otro usuario.", ErrorType.Conflict);
        }

        if (await _uow.Usuarios.ExistsByNombreUsuario(nombreUsuario, request.UsuarioId))
        {
            return ApiResponse<UsuarioResponseDto>.Fail($"El nombre {nombreUsuario} ya esta en uso por otro usuario.", ErrorType.Conflict);
        }

        entity.NombreUsuario = nombreUsuario;
        entity.Email         = email;
        entity.Activo        = request.Activo;
        entity.UpdatedAt     = DateTime.UtcNow;

        // El rol solo cambia si se envia; si se omite se conserva el actual.
        if (!string.IsNullOrWhiteSpace(request.Rol))
        {
            Enum.TryParse<RolUsuario>(request.Rol, ignoreCase: true, out var parsedRol);
            entity.Rol = parsedRol;
        }

        if (!string.IsNullOrWhiteSpace(request.NuevaContrasenia))
        {
            entity.Contrasenia = _passwordService.Hash(request.NuevaContrasenia);
        }

        await _uow.Usuarios.Update(entity);
        await _uow.Complete();

        return ApiResponse<UsuarioResponseDto>.Ok(new UsuarioResponseDto
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
