using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Queries;

public class GetAllCategoriasQuery : IRequest<ApiPaginationResponse<CategoriaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllCategoriasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllCategoriasQuery, ApiPaginationResponse<CategoriaResponseDto>>
{
    public async Task<ApiPaginationResponse<CategoriaResponseDto>> Handle(GetAllCategoriasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<CategoriaEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<CategoriaResponseDto>.Fail("No se encontraron categorías.");
        }

        return ApiPaginationResponse<CategoriaResponseDto>.Ok(items.Select(x => new CategoriaResponseDto
        {
            Id = x.Id,
            NomCategoria = x.NomCategoria
        }), total, request.Page, request.PageSize);
    }
}
