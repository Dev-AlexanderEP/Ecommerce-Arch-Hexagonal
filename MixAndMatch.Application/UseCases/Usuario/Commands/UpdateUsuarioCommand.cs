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
    public required long UsuarioId { get; set; }
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
        var repo   = _uow.Repository<UsuarioEntity>();
        var entity = await repo.GetById(request.UsuarioId);

        if (entity is null)
            return ApiResponse<UsuarioResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");

        var emailTaken = (await repo.GetAll())
            .Any(u => u.Email == request.Email && u.Id != request.UsuarioId);
        if (emailTaken)
            return ApiResponse<UsuarioResponseDto>.Fail($"El email {request.Email} ya está en uso por otro usuario.");

        if (request.Rol is not null && !Roles.IsValid(request.Rol))
            return ApiResponse<UsuarioResponseDto>.Fail($"Rol inválido: {request.Rol}. Roles permitidos: {string.Join(", ", Roles.All)}.");

        entity.NombreUsuario = request.NombreUsuario;
        entity.Email         = request.Email;
        entity.Rol           = request.Rol;
        entity.Activo        = request.Activo;
        entity.UpdatedAt     = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.NuevaContrasenia))
            entity.Contrasenia = _passwordService.Hash(request.NuevaContrasenia);

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<UsuarioResponseDto>.Ok(new UsuarioResponseDto
        {
            Id            = entity.Id,
            NombreUsuario = entity.NombreUsuario,
            Email         = entity.Email,
            Rol           = entity.Rol,
            Activo        = entity.Activo,
            CreatedAt     = entity.CreatedAt,
            UpdatedAt     = entity.UpdatedAt
        });
    }
}
