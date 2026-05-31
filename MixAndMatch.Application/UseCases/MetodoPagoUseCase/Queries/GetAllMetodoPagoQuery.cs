using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Queries;

public class GetAllMetodoPagoQuery : IRequest<ApiResponseDto<IEnumerable<MetodoPagoResponseDto>>>
{
}

public class GetAllMetodoPagoQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllMetodoPagoQuery, ApiResponseDto<IEnumerable<MetodoPagoResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<MetodoPagoResponseDto>>> Handle(
        GetAllMetodoPagoQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _uow.Repository<Domain.Entities.MetodoPago>()
                .GetAll();

            var result = entities.Select(x => new MetodoPagoResponseDto
            {
                Id = x.Id,
                TipoPago = x.TipoPago,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            });

            return ApiResponseDto<IEnumerable<MetodoPagoResponseDto>>
                .Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<MetodoPagoResponseDto>>
                .Fail(ex.Message);
        }
    }
}