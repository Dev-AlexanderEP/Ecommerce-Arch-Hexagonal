using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Usuario.Queries;

public class GetUsuarioByIdQuery : IRequest<ApiResponse<UsuarioResponseDto>>
{
    public required long UsuarioId { get; set; }
}

public class GetUsuarioByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetUsuarioByIdQuery, ApiResponse<UsuarioResponseDto>>
{
    public async Task<ApiResponse<UsuarioResponseDto>> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Usuarios.GetById(request.UsuarioId);
        if (entity is null)
        {
            return ApiResponse<UsuarioResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

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
