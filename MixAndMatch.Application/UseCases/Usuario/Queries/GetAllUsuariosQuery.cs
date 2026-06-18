using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

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
        var (items, total) = await _uow.Usuarios.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<UsuarioResponseDto>.Ok(items.Select(u => new UsuarioResponseDto
        {
            Id            = u.Id,
            NombreUsuario = u.NombreUsuario,
            Email         = u.Email,
            Rol           = u.Rol?.ToString(),
            Activo        = u.Activo,
            CreatedAt     = u.CreatedAt,
            UpdatedAt     = u.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
