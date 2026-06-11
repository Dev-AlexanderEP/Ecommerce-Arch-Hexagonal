using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

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
        var (items, total) = await _uow.Repository<PrendaTallaEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
            return ApiPaginationResponse<PrendaTallaResponseDto>.Fail("No se encontraron combinaciones de prenda y talla.");

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
