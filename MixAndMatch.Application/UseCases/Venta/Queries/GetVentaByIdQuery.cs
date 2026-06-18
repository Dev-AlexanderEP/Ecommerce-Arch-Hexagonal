using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetVentaByIdQuery : IRequest<ApiResponse<VentaResponseDto>>
{
    public required long VentaId { get; set; }
    public required long SolicitanteId { get; set; }
    public required bool EsAdmin { get; set; }
}

public class GetVentaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetVentaByIdQuery, ApiResponse<VentaResponseDto>>
{
    public async Task<ApiResponse<VentaResponseDto>> Handle(GetVentaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Ventas.GetById(request.VentaId);
        if (entity is null)
        {
            return ApiResponse<VentaResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        // El ADMIN ve cualquier venta; el CLIENTE solo las suyas.
        if (!request.EsAdmin && entity.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<VentaResponseDto>.Fail("No tienes acceso a esta venta.", ErrorType.Forbidden);
        }

        return ApiResponse<VentaResponseDto>.Ok(new VentaResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado?.ToString(),
            UpdatedAt = entity.UpdatedAt
        });
    }
}
