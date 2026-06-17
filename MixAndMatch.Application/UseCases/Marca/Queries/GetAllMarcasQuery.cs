using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Marca.Queries;

public class GetAllMarcasQuery : IRequest<ApiPaginationResponse<MarcaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllMarcasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllMarcasQuery, ApiPaginationResponse<MarcaResponseDto>>
{
    public async Task<ApiPaginationResponse<MarcaResponseDto>> Handle(GetAllMarcasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Marcas.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<MarcaResponseDto>.Ok(items.Select(x => new MarcaResponseDto
        {
            Id = x.Id,
            NomMarca = x.NomMarca
        }), total, request.Page, request.PageSize);
    }
}
