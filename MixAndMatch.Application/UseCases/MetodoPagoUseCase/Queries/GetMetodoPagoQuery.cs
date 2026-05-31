using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Queries;

public class GetMetodoPagoByIdQuery : IRequest<ApiResponseDto<MetodoPagoResponseDto>>
{
    public long Id { get; set; }
}

public class GetMetodoPagoByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetMetodoPagoByIdQuery, ApiResponseDto<MetodoPagoResponseDto>>
{
    public async Task<ApiResponseDto<MetodoPagoResponseDto>> Handle(
        GetMetodoPagoByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<Domain.Entities.MetodoPago>()
                .GetById(request.Id);

            if (entity is null)
            {
                return ApiResponseDto<MetodoPagoResponseDto>
                    .Fail($"Método de pago no encontrado para id {request.Id}");
            }

            return ApiResponseDto<MetodoPagoResponseDto>.Ok(
                new MetodoPagoResponseDto
                {
                    Id = entity.Id,
                    TipoPago = entity.TipoPago,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                });
        }
        catch (Exception ex)
        {
            return ApiResponseDto<MetodoPagoResponseDto>
                .Fail(ex.Message);
        }
    }
}