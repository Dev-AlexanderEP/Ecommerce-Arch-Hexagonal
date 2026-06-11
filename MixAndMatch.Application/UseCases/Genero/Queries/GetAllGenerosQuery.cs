using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;

namespace MixAndMatch.Application.UseCases.Genero.Queries;

public class GetAllGenerosQuery : IRequest<ApiPaginationResponse<GeneroResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllGenerosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllGenerosQuery, ApiPaginationResponse<GeneroResponseDto>>
{
    public async Task<ApiPaginationResponse<GeneroResponseDto>> Handle(GetAllGenerosQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<GeneroEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<GeneroResponseDto>.Fail("No se encontraron géneros.");
        }

        return ApiPaginationResponse<GeneroResponseDto>.Ok(items.Select(x => new GeneroResponseDto
        {
            Id = x.Id,
            NomGenero = x.NomGenero
        }), total, request.Page, request.PageSize);
    }
}
