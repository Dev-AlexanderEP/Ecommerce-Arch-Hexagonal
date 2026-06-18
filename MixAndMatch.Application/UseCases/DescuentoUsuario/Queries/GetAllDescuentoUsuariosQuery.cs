using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Queries;

public class GetAllDescuentoUsuariosQuery : IRequest<ApiPaginationResponse<DescuentoUsuarioResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public required long SolicitanteId { get; set; }
}

public class GetAllDescuentoUsuariosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoUsuariosQuery, ApiPaginationResponse<DescuentoUsuarioResponseDto>>
{
    public async Task<ApiPaginationResponse<DescuentoUsuarioResponseDto>> Handle(GetAllDescuentoUsuariosQuery request, CancellationToken cancellationToken)
    {
        // Solo los registros de uso del propio solicitante (ownership).
        var (items, total) = await _uow.DescuentoUsuarios.GetPagedByUsuario(request.SolicitanteId, request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<DescuentoUsuarioResponseDto>.Ok(items.Select(x => new DescuentoUsuarioResponseDto
        {
            Id = x.Id,
            DescuentoCodigoId = x.DescuentoCodigoId,
            UsuarioId = x.UsuarioId,
            FechaUso = x.FechaUso,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
