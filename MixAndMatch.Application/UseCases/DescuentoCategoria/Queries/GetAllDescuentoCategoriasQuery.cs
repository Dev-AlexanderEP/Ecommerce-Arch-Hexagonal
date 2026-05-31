using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Queries;

public class GetAllDescuentoCategoriasQuery : IRequest<ApiResponseDto<IEnumerable<DescuentoCategoriaResponseDto>>>
{
}

public class GetAllDescuentoCategoriasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoCategoriasQuery, ApiResponseDto<IEnumerable<DescuentoCategoriaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<DescuentoCategoriaResponseDto>>> Handle(GetAllDescuentoCategoriasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<DescuentoCategoriaEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<DescuentoCategoriaResponseDto>>.Fail("No se encontraron descuentos por categoría.");
        }

        return ApiResponseDto<IEnumerable<DescuentoCategoriaResponseDto>>.Ok(items.Select(x => new DescuentoCategoriaResponseDto
        {
            Id = x.Id,
            CategoriaId = x.CategoriaId,
            Porcentaje = x.Porcentaje,
            FechaInicio = x.FechaInicio,
            FechaFin = x.FechaFin,
            Activo = x.Activo,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
