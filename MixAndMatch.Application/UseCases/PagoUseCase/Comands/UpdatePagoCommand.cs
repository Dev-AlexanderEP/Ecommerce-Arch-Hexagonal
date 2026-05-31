using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;
using MetodoPagoEntity = MixAndMatch.Domain.Entities.MetodoPago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class UpdatePagoCommand : IRequest<ApiResponseDto<PagoResponseDto>>
{
    public long Id { get; set; }
    public long VentaId { get; set; }
    public long MetodoId { get; set; }
    public decimal Monto { get; set; }
    public required string Estado { get; set; }
}

public class UpdatePagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdatePagoCommand, ApiResponseDto<PagoResponseDto>>
{
    public async Task<ApiResponseDto<PagoResponseDto>> Handle(
        UpdatePagoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<PagoEntity>()
                .GetById(request.Id);

            if (entity is null)
                return ApiResponseDto<PagoResponseDto>
                    .Fail($"Pago no encontrado para id {request.Id}");

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

            entity.VentaId = request.VentaId;
            entity.MetodoId = request.MetodoId;
            entity.Monto = request.Monto;
            entity.Estado = request.Estado;
            entity.UpdatedAt = DateTime.UtcNow;

            await _uow.Repository<PagoEntity>().Update(entity);
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