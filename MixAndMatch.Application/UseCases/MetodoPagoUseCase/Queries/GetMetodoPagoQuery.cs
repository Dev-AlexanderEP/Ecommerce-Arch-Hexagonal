using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Queries;

public class GetMetodoPagoByIdQuery : IRequest<ApiResponse<MetodoPagoResponseDto>>
{
    public long Id { get; set; }
}

public class GetMetodoPagoByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetMetodoPagoByIdQuery, ApiResponse<MetodoPagoResponseDto>>
{
    public async Task<ApiResponse<MetodoPagoResponseDto>> Handle(
        GetMetodoPagoByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<Domain.Entities.MetodoPago>()
                .GetById(request.Id);

            if (entity is null)
            {
                return ApiResponse<MetodoPagoResponseDto>
                    .Fail($"MÃ©todo de pago no encontrado para id {request.Id}");
            }

            return ApiResponse<MetodoPagoResponseDto>.Ok(
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
            return ApiResponse<MetodoPagoResponseDto>
                .Fail(ex.Message);
        }
    }
}