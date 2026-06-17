using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Queries;

public class GetAllMetodoPagoQuery : IRequest<ApiPaginationResponse<MetodoPagoResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllMetodoPagoQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllMetodoPagoQuery, ApiPaginationResponse<MetodoPagoResponseDto>>
{
    public async Task<ApiPaginationResponse<MetodoPagoResponseDto>> Handle(GetAllMetodoPagoQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.MetodoPagos.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<MetodoPagoResponseDto>.Ok(items.Select(x => new MetodoPagoResponseDto
        {
            Id = x.Id,
            TipoPago = x.TipoPago,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
