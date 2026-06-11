using MixAndMatch.Domain.DTOs.MetodoPago;

namespace MixAndMatch.Application.UseCases.MetodoPagoUseCase.Commands;



using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MetodoPagoEntity = MixAndMatch.Domain.Entities.MetodoPago;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;



public class CreateMetodoPagoCommand : IRequest<ApiResponse<PagoResponseDto>>
{
    public required long VentaId { get; set; }
    public required long MetodoId { get; set; }
    public required decimal Monto { get; set; }
    public required string Estado { get; set; }
}

public class CreatePagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateMetodoPagoCommand, ApiResponse<PagoResponseDto>>
{
    public async Task<ApiResponse<PagoResponseDto>> Handle(CreateMetodoPagoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var venta = await _uow.Repository<VentaEntity>()
                .GetById(request.VentaId);

            if (venta is null)
            {
                return ApiResponse<PagoResponseDto>
                    .Fail($"No existe la venta con id {request.VentaId}.");
            }

            var metodo = await _uow.Repository<MetodoPagoEntity>()
                .GetById(request.MetodoId);

            if (metodo is null)
            {
                return ApiResponse<PagoResponseDto>
                    .Fail($"No existe el mÃ©todo de pago con id {request.MetodoId}.");
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

            return ApiResponse<PagoResponseDto>.Ok(
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
            return ApiResponse<PagoResponseDto>
                .Fail($"Error al registrar el pago: {ex.Message}");
        }
    }
}