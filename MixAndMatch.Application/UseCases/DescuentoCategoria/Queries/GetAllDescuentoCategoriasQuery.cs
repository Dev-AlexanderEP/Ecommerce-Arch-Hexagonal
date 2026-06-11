using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Queries;

public class GetAllDescuentoCategoriasQuery : IRequest<ApiPaginationResponse<DescuentoCategoriaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllDescuentoCategoriasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoCategoriasQuery, ApiPaginationResponse<DescuentoCategoriaResponseDto>>
{
    public async Task<ApiPaginationResponse<DescuentoCategoriaResponseDto>> Handle(GetAllDescuentoCategoriasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<DescuentoCategoriaEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<DescuentoCategoriaResponseDto>.Fail("No se encontraron descuentos por categoría.");
        }

        return ApiPaginationResponse<DescuentoCategoriaResponseDto>.Ok(items.Select(x => new DescuentoCategoriaResponseDto
        {
            Id = x.Id,
            CategoriaId = x.CategoriaId,
            Porcentaje = x.Porcentaje,
            FechaInicio = x.FechaInicio,
            FechaFin = x.FechaFin,
            Activo = x.Activo,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
