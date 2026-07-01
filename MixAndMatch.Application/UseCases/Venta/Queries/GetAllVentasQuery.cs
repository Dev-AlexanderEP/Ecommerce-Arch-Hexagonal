using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetAllVentasQuery : IRequest<ApiPaginationResponse<VentaResponseDto>>
{
    public string? NombreUsuario { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllVentasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllVentasQuery, ApiPaginationResponse<VentaResponseDto>>
{
    public async Task<ApiPaginationResponse<VentaResponseDto>> Handle(GetAllVentasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Ventas.GetPagedConFiltro(request.NombreUsuario, request.Page, request.PageSize);

        return ApiPaginationResponse<VentaResponseDto>.Ok(items.Select(x => new VentaResponseDto
        {
            Id = x.Id,
            UsuarioId = x.UsuarioId,
            NombreUsuario = x.Usuario?.NombreUsuario,
            FechaCreacion = x.FechaCreacion,
            Estado = x.Estado?.ToString(),
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
