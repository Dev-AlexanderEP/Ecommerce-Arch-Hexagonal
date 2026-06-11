using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Queries;

public class GetAllPrendaImagenesQuery : IRequest<ApiPaginationResponse<PrendaImagenResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllPrendaImagenesQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllPrendaImagenesQuery, ApiPaginationResponse<PrendaImagenResponseDto>>
{
    public async Task<ApiPaginationResponse<PrendaImagenResponseDto>> Handle(GetAllPrendaImagenesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<PrendaImagenEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
            return ApiPaginationResponse<PrendaImagenResponseDto>.Fail("No se encontraron imágenes de prendas.");

        return ApiPaginationResponse<PrendaImagenResponseDto>.Ok(items.Select(x => new PrendaImagenResponseDto
        {
            Id = x.Id,
            PrendaId = x.PrendaId,
            Tipo = x.Tipo,
            Url = x.Url,
            Orden = x.Orden,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
