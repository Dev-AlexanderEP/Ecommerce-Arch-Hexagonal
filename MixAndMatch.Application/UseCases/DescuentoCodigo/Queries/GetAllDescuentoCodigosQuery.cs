using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCodigoEntity = MixAndMatch.Domain.Entities.DescuentoCodigo;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Queries;

public class GetAllDescuentoCodigosQuery : IRequest<ApiResponseDto<IEnumerable<DescuentoCodigoResponseDto>>>
{
}

public class GetAllDescuentoCodigosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoCodigosQuery, ApiResponseDto<IEnumerable<DescuentoCodigoResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<DescuentoCodigoResponseDto>>> Handle(GetAllDescuentoCodigosQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<DescuentoCodigoEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<DescuentoCodigoResponseDto>>.Fail("No se encontraron descuentos por código.");
        }

        return ApiResponseDto<IEnumerable<DescuentoCodigoResponseDto>>.Ok(items.Select(x => new DescuentoCodigoResponseDto
        {
            Id = x.Id,
            Codigo = x.Codigo,
            Descripcion = x.Descripcion,
            Porcentaje = x.Porcentaje,
            FechaInicio = x.FechaInicio,
            FechaFin = x.FechaFin,
            UsoMaximo = x.UsoMaximo,
            Activo = x.Activo,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
