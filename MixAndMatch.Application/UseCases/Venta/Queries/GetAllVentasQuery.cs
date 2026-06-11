using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetAllVentasQuery : IRequest<ApiPaginationResponse<VentaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllVentasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllVentasQuery, ApiPaginationResponse<VentaResponseDto>>
{
    public async Task<ApiPaginationResponse<VentaResponseDto>> Handle(GetAllVentasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<VentaEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<VentaResponseDto>.Fail("No se encontraron ventas.");
        }

        return ApiPaginationResponse<VentaResponseDto>.Ok(items.Select(x => new VentaResponseDto
        {
            Id = x.Id,
            UsuarioId = x.UsuarioId,
            FechaCreacion = x.FechaCreacion,
            Estado = x.Estado,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
