using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;
using MetodoPagoEntity = MixAndMatch.Domain.Entities.MetodoPago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class CreatePagoCommand : IRequest<ApiResponseDto<PagoResponseDto>>
{
    public long VentaId { get; set; }
    public long MetodoId { get; set; }
    public decimal Monto { get; set; }
    public required string Estado { get; set; }
}

public class CreatePagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreatePagoCommand, ApiResponseDto<PagoResponseDto>>
{
    public async Task<ApiResponseDto<PagoResponseDto>> Handle(
        CreatePagoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var venta = await _uow.Repository<VentaEntity>()
                .GetById(request.VentaId);

            if (venta is null)
                return ApiResponseDto<PagoResponseDto>
                    .Fail($"Venta no encontrada para id {request.VentaId}");

            var metodo = await _uow.Repository<MetodoPagoEntity>()
                .GetById(request.MetodoId);

            if (metodo is null)
                return ApiResponseDto<PagoResponseDto>
                    .Fail($"Método de pago no encontrado para id {request.MetodoId}");

            var entity = new PagoEntity
            {
                VentaId = request.VentaId,
                MetodoId = request.MetodoId,
                Monto = request.Monto,
                Estado = request.Estado,
                FechaCreacion = DateTime.UtcNow
            };

            await _uow.Repository<PagoEntity>().Add(entity);
            await _uow.Complete();

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