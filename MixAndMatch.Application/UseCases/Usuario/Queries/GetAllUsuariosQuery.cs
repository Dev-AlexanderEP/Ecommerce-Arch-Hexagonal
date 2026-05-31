using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Usuario.Queries;

public class GetAllUsuariosQuery : IRequest<ApiResponseDto<IEnumerable<UsuarioResponseDto>>>
{
}

public class GetAllUsuariosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllUsuariosQuery, ApiResponseDto<IEnumerable<UsuarioResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<UsuarioResponseDto>>> Handle(GetAllUsuariosQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<UsuarioEntity>().GetAll();

        if (!items.Any())
            return ApiResponseDto<IEnumerable<UsuarioResponseDto>>.Fail("No se encontraron usuarios.");

        return ApiResponseDto<IEnumerable<UsuarioResponseDto>>.Ok(items.Select(u => new UsuarioResponseDto
        {
            Id            = u.Id,
            NombreUsuario = u.NombreUsuario,
            Email         = u.Email,
            Rol           = u.Rol,
            Activo        = u.Activo,
            CreatedAt     = u.CreatedAt,
            UpdatedAt     = u.UpdatedAt
        }));
    }
}
