using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

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
        var (items, total) = await _uow.Generos.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<GeneroResponseDto>.Ok(items.Select(x => new GeneroResponseDto
        {
            Id = x.Id,
            NomGenero = x.NomGenero
        }), total, request.Page, request.PageSize);
    }
}
