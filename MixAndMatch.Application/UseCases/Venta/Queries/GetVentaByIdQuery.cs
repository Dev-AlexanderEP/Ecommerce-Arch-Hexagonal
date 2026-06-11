using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetVentaByIdQuery : IRequest<ApiResponse<VentaResponseDto>>
{
    public required long VentaId { get; set; }
}

public class GetVentaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetVentaByIdQuery, ApiResponse<VentaResponseDto>>
{
    public async Task<ApiResponse<VentaResponseDto>> Handle(GetVentaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<VentaEntity>().GetById(request.VentaId);
        if (entity is null)
        {
            return ApiResponse<VentaResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        return ApiResponse<VentaResponseDto>.Ok(new VentaResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
