using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Queries;

public class GetPagoByIdQuery : IRequest<ApiResponseDto<PagoResponseDto>>
{
    public long Id { get; set; }
}

public class GetPagoByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetPagoByIdQuery, ApiResponseDto<PagoResponseDto>>
{
    public async Task<ApiResponseDto<PagoResponseDto>> Handle(
        GetPagoByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<PagoEntity>()
                .GetById(request.Id);

            if (entity is null)
                return ApiResponseDto<PagoResponseDto>
                    .Fail($"Pago no encontrado para id {request.Id}");

            return ApiResponseDto<PagoResponseDto>.Ok(
                new PagoResponseDto
                {
                    Id = entity.Id,
                    VentaId = entity.VentaId,
                    MetodoId = entity.MetodoId,
                    Monto = entity.Monto,
                    Estado = entity.Estado,
                    FechaCreacion = entity.FechaCreacion,
                    UpdatedAt = entity.UpdatedAt
                });
        }
        catch (Exception ex)
        {
            return ApiResponseDto<PagoResponseDto>.Fail(ex.Message);
        }
    }
}