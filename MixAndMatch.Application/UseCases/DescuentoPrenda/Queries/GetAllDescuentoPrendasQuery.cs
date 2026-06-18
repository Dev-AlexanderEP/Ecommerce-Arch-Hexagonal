using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Queries;

public class GetAllDescuentoPrendasQuery : IRequest<ApiPaginationResponse<DescuentoPrendaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllDescuentoPrendasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoPrendasQuery, ApiPaginationResponse<DescuentoPrendaResponseDto>>
{
    public async Task<ApiPaginationResponse<DescuentoPrendaResponseDto>> Handle(GetAllDescuentoPrendasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<DescuentoPrendaEntity>().GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<DescuentoPrendaResponseDto>.Ok(items.Select(x => new DescuentoPrendaResponseDto
        {
            Id = x.Id,
            PrendaId = x.PrendaId,
            Porcentaje = x.Porcentaje,
            FechaInicio = x.FechaInicio,
            FechaFin = x.FechaFin,
            Activo = x.Activo,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
