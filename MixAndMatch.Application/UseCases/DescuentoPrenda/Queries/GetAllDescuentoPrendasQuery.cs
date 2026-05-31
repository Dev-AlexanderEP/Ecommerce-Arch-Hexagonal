using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Queries;

public class GetAllDescuentoPrendasQuery : IRequest<ApiResponseDto<IEnumerable<DescuentoPrendaResponseDto>>>
{
}

public class GetAllDescuentoPrendasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoPrendasQuery, ApiResponseDto<IEnumerable<DescuentoPrendaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<DescuentoPrendaResponseDto>>> Handle(GetAllDescuentoPrendasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<DescuentoPrendaEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<DescuentoPrendaResponseDto>>.Fail("No se encontraron descuentos por prenda.");
        }

        return ApiResponseDto<IEnumerable<DescuentoPrendaResponseDto>>.Ok(items.Select(x => new DescuentoPrendaResponseDto
        {
            Id = x.Id,
            PrendaId = x.PrendaId,
            Porcentaje = x.Porcentaje,
            FechaInicio = x.FechaInicio,
            FechaFin = x.FechaFin,
            Activo = x.Activo,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
