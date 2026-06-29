using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Queries;

public class GetDescuentoCodigoByCodigoQuery : IRequest<ApiResponse<DescuentoCodigoResponseDto>>
{
    public required string Codigo { get; set; }
}

public class GetDescuentoCodigoByCodigoQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetDescuentoCodigoByCodigoQuery, ApiResponse<DescuentoCodigoResponseDto>>
{
    public async Task<ApiResponse<DescuentoCodigoResponseDto>> Handle(
        GetDescuentoCodigoByCodigoQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.DescuentoCodigos.BuscarPorCodigo(request.Codigo);
        if (entity is null)
            return ApiResponse<DescuentoCodigoResponseDto>.Fail($"Código de descuento '{request.Codigo}' no encontrado.", ErrorType.NotFound);

        return ApiResponse<DescuentoCodigoResponseDto>.Ok(new DescuentoCodigoResponseDto
        {
            Id          = entity.Id,
            Codigo      = entity.Codigo,
            Descripcion = entity.Descripcion,
            Porcentaje  = entity.Porcentaje,
            FechaInicio = entity.FechaInicio,
            FechaFin    = entity.FechaFin,
            UsoMaximo   = entity.UsoMaximo,
            Activo      = entity.Activo,
            CreatedAt   = entity.CreatedAt,
            UpdatedAt   = entity.UpdatedAt
        });
    }
}
