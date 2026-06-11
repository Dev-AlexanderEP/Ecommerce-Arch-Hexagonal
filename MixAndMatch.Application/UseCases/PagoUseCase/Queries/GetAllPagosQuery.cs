using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Queries;

public class GetAllPagosQuery : IRequest<ApiPaginationResponse<PagoResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllPagosQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllPagosQuery, ApiPaginationResponse<PagoResponseDto>>
{
    public async Task<ApiPaginationResponse<PagoResponseDto>> Handle(
        GetAllPagosQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (entities, total) = await _uow.Repository<PagoEntity>()
                .GetPaged(request.Page, request.PageSize);

            var result = entities.Select(x => new PagoResponseDto
            {
                Id = x.Id,
                VentaId = x.VentaId,
                MetodoId = x.MetodoId,
                Monto = x.Monto,
                Estado = x.Estado,
                FechaCreacion = x.FechaCreacion,
                UpdatedAt = x.UpdatedAt
            });

            return ApiPaginationResponse<PagoResponseDto>
                .Ok(result, total, request.Page, request.PageSize);
        }
        catch (Exception ex)
        {
            return ApiPaginationResponse<PagoResponseDto>
                .Fail(ex.Message);
        }
    }
}