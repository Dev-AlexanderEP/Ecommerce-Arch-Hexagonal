using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
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
    public async Task<ApiPaginationResponse<MetodoPagoResponseDto>> Handle(
        GetAllMetodoPagoQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entities, total) = await _uow.Repository<Domain.Entities.MetodoPago>()
                .GetPaged(request.Page, request.PageSize);

            var result = entities.Select(x => new MetodoPagoResponseDto
            {
                Id = x.Id,
                TipoPago = x.TipoPago,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            });

            return ApiPaginationResponse<MetodoPagoResponseDto>
                .Ok(result, total, request.Page, request.PageSize);
        }
        catch (Exception ex)
        {
            return ApiPaginationResponse<MetodoPagoResponseDto>
                .Fail(ex.Message);
        }
    }
}