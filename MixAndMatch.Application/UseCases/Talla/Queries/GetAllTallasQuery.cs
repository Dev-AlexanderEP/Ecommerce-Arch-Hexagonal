using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Queries;

public class GetAllTallasQuery : IRequest<ApiPaginationResponse<TallaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllTallasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllTallasQuery, ApiPaginationResponse<TallaResponseDto>>
{
    public async Task<ApiPaginationResponse<TallaResponseDto>> Handle(GetAllTallasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<TallaEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
            return ApiPaginationResponse<TallaResponseDto>.Fail("No se encontraron tallas.");

        return ApiPaginationResponse<TallaResponseDto>.Ok(
            items.Select(x => new TallaResponseDto { Id = x.Id, NomTalla = x.NomTalla }), total, request.Page, request.PageSize);
    }
}
