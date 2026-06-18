using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Queries;

public class GetAllDescuentoCodigosQuery : IRequest<ApiPaginationResponse<DescuentoCodigoResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllDescuentoCodigosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoCodigosQuery, ApiPaginationResponse<DescuentoCodigoResponseDto>>
{
    public async Task<ApiPaginationResponse<DescuentoCodigoResponseDto>> Handle(GetAllDescuentoCodigosQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.DescuentoCodigos.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<DescuentoCodigoResponseDto>.Ok(items.Select(x => new DescuentoCodigoResponseDto
        {
            Id = x.Id,
            Codigo = x.Codigo,
            Descripcion = x.Descripcion,
            Porcentaje = x.Porcentaje,
            FechaInicio = x.FechaInicio,
            FechaFin = x.FechaFin,
            UsoMaximo = x.UsoMaximo,
            Activo = x.Activo,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
