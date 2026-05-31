using MixAndMatch.Domain.DTOs.MetodoPago;

namespace MixAndMatch.Application.UseCases.MetodoPagoUseCase.Commands;



using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MetodoPagoEntity = MixAndMatch.Domain.Entities.MetodoPago;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;



public class CreateMetodoPagoCommand : IRequest<ApiResponseDto<PagoResponseDto>>
{
    public required long VentaId { get; set; }
    public required long MetodoId { get; set; }
    public required decimal Monto { get; set; }
    public required string Estado { get; set; }
}

public class CreatePagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateMetodoPagoCommand, ApiResponseDto<PagoResponseDto>>
{
    public async Task<ApiResponseDto<PagoResponseDto>> Handle(CreateMetodoPagoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var venta = await _uow.Repository<VentaEntity>()
                .GetById(request.VentaId);

            if (venta is null)
            {
                return ApiResponseDto<PagoResponseDto>
                    .Fail($"No existe la venta con id {request.VentaId}.");
            }

            var metodo = await _uow.Repository<MetodoPagoEntity>()
                .GetById(request.MetodoId);

            if (metodo is null)
            {
                return ApiResponseDto<PagoResponseDto>
                    .Fail($"No existe el método de pago con id {request.MetodoId}.");
            }

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
            return ApiResponseDto<PagoResponseDto>
                .Fail($"Error al registrar el pago: {ex.Message}");
        }
    }
}