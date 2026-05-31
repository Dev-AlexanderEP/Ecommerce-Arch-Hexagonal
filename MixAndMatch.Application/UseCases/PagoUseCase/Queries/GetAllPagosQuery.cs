using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Queries;

public class GetAllPagosQuery : IRequest<ApiResponseDto<IEnumerable<PagoResponseDto>>>
{
}

public class GetAllPagosQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllPagosQuery, ApiResponseDto<IEnumerable<PagoResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<PagoResponseDto>>> Handle(
        GetAllPagosQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _uow.Repository<PagoEntity>()
                .GetAll();

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

            return ApiResponseDto<IEnumerable<PagoResponseDto>>
                .Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<PagoResponseDto>>
                .Fail(ex.Message);
        }
    }
}