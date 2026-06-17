using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Queries;

public class GetPagoByIdQuery : IRequest<ApiResponse<PagoResponseDto>>
{
    public required long Id { get; set; }
    public required long SolicitanteId { get; set; }
    public required bool EsAdmin { get; set; }
}

public class GetPagoByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetPagoByIdQuery, ApiResponse<PagoResponseDto>>
{
    public async Task<ApiResponse<PagoResponseDto>> Handle(GetPagoByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<PagoEntity>().GetById(request.Id);
        if (entity is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Pago no encontrado para id {request.Id}.");
        }

        // El ADMIN ve cualquier pago; el CLIENTE solo los de sus propias ventas.
        if (!request.EsAdmin)
        {
            var venta = await _uow.Ventas.GetById(entity.VentaId);
            if (venta is null || venta.UsuarioId != request.SolicitanteId)
            {
                return ApiResponse<PagoResponseDto>.Fail("No tienes acceso a este pago.", ErrorType.Forbidden);
            }
        }

        return ApiResponse<PagoResponseDto>.Ok(new PagoResponseDto
        {
            Id = entity.Id,
            VentaId = entity.VentaId,
            MetodoId = entity.MetodoId,
            Monto = entity.Monto,
            Estado = entity.Estado.ToString(),
            FechaCreacion = entity.FechaCreacion,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
