using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Usuario.Queries;

public class GetUsuarioByIdQuery : IRequest<ApiResponseDto<UsuarioResponseDto>>
{
    public required long UsuarioId { get; set; }
}

public class GetUsuarioByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetUsuarioByIdQuery, ApiResponseDto<UsuarioResponseDto>>
{
    public async Task<ApiResponseDto<UsuarioResponseDto>> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<UsuarioEntity>().GetById(request.UsuarioId);

        if (entity is null)
            return ApiResponseDto<UsuarioResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");

        return ApiResponseDto<UsuarioResponseDto>.Ok(new UsuarioResponseDto
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
