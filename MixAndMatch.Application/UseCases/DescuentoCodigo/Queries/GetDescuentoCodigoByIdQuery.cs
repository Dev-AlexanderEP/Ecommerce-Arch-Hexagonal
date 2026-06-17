using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Queries;

public class GetDescuentoCodigoByIdQuery : IRequest<ApiResponse<DescuentoCodigoResponseDto>>
{
    public required long DescuentoCodigoId { get; set; }
}

public class GetDescuentoCodigoByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentoCodigoByIdQuery, ApiResponse<DescuentoCodigoResponseDto>>
{
    public async Task<ApiResponse<DescuentoCodigoResponseDto>> Handle(GetDescuentoCodigoByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.DescuentoCodigos.GetById(request.DescuentoCodigoId);
        if (entity is null)
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail($"Descuento de código no encontrado para id {request.DescuentoCodigoId}.");
        }

        return ApiResponse<DescuentoCodigoResponseDto>.Ok(new DescuentoCodigoResponseDto
        {
            Id = entity.Id,
            Codigo = entity.Codigo,
            Descripcion = entity.Descripcion,
            Porcentaje = entity.Porcentaje,
            FechaInicio = entity.FechaInicio,
            FechaFin = entity.FechaFin,
            UsoMaximo = entity.UsoMaximo,
            Activo = entity.Activo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
