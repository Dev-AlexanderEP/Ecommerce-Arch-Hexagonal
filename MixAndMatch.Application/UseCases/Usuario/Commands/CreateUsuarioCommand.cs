using MediatR;
using MixAndMatch.Application.Common;
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

public class CreateUsuarioCommandHandler(IUnitOfWork _uow, IPasswordService _passwordService)
    : IRequestHandler<CreateUsuarioCommand, ApiResponse<UsuarioResponseDto>>
{
    public async Task<ApiResponse<UsuarioResponseDto>> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<UsuarioEntity>();

        var existing = (await repo.GetAll()).FirstOrDefault(u => u.Email == request.Email);
        if (existing is not null)
            return ApiResponse<UsuarioResponseDto>.Fail($"Ya existe un usuario con el email {request.Email}.");

        var entity = new UsuarioEntity
        {
            NombreUsuario = request.NombreUsuario,
            Email         = request.Email,
            Contrasenia   = _passwordService.Hash(request.Contrasenia),
            Rol           = request.Rol,
            Activo        = request.Activo,
            CreatedAt     = DateTime.UtcNow
        };

        await repo.Add(entity);
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
