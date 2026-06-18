using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Queries;

public class GetAllPrendaTallasQuery : IRequest<ApiPaginationResponse<PrendaTallaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllPrendaTallasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllPrendaTallasQuery, ApiPaginationResponse<PrendaTallaResponseDto>>
{
    public async Task<ApiPaginationResponse<PrendaTallaResponseDto>> Handle(GetAllPrendaTallasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.PrendaTallas.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<PrendaTallaResponseDto>.Ok(items.Select(x => new PrendaTallaResponseDto
        {
            Id = x.Id,
            PrendaId = x.PrendaId,
            TallaId = x.TallaId,
            Stock = x.Stock,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
