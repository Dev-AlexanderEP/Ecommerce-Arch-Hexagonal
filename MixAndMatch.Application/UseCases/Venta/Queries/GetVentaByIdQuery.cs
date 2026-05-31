using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetVentaByIdQuery : IRequest<ApiResponseDto<VentaResponseDto>>
{
    public required long VentaId { get; set; }
}

public class GetVentaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetVentaByIdQuery, ApiResponseDto<VentaResponseDto>>
{
    public async Task<ApiResponseDto<VentaResponseDto>> Handle(GetVentaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<VentaEntity>().GetById(request.VentaId);
        if (entity is null)
        {
            return ApiResponseDto<VentaResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        return ApiResponseDto<VentaResponseDto>.Ok(new VentaResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
