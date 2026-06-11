
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Commands;

public class UpdateMetodoPagoCommand : IRequest<ApiResponse<MetodoPagoResponseDto>>
{
    public long Id { get; set; }
    public required string TipoPago { get; set; }
}

public class UpdateMetodoPagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdateMetodoPagoCommand, ApiResponse<MetodoPagoResponseDto>>
{
    public async Task<ApiResponse<MetodoPagoResponseDto>> Handle(
        UpdateMetodoPagoCommand request,
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

            entity.TipoPago = request.TipoPago;
            entity.UpdatedAt = DateTime.UtcNow;

            await _uow.Repository<Domain.Entities.MetodoPago>().Update(entity);
            await _uow.Complete();

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