using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Usuario.Queries;

public class GetAllUsuariosQuery : IRequest<ApiPaginationResponse<UsuarioResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllUsuariosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllUsuariosQuery, ApiPaginationResponse<UsuarioResponseDto>>
{
    public async Task<ApiPaginationResponse<UsuarioResponseDto>> Handle(GetAllUsuariosQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<UsuarioEntity>().GetPaged(request.Page, request.PageSize);

        if (!items.Any())
            return ApiPaginationResponse<UsuarioResponseDto>.Fail("No se encontraron usuarios.");

        return ApiPaginationResponse<UsuarioResponseDto>.Ok(items.Select(u => new UsuarioResponseDto
        {
            Id            = u.Id,
            NombreUsuario = u.NombreUsuario,
            Email         = u.Email,
            Rol           = u.Rol,
            Activo        = u.Activo,
            CreatedAt     = u.CreatedAt,
            UpdatedAt     = u.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
