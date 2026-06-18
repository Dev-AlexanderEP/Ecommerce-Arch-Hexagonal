using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniasByPrendaQuery : IRequest<ApiResponse<ReseniaResumenDto>>
{
    public required long PrendaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReseniasByPrendaQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetReseniasByPrendaQuery, ApiResponse<ReseniaResumenDto>>
{
    public async Task<ApiResponse<ReseniaResumenDto>> Handle(GetReseniasByPrendaQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _uow.Resenias.GetPaginatedByPrendaIdAsync(request.PrendaId, page, pageSize);
        var promedio = await _uow.Resenias.GetPromedioByPrendaIdAsync(request.PrendaId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Una prenda sin resenias no es un error: 200 con lista vacia y promedio 0.
        return ApiResponse<ReseniaResumenDto>.Ok(new ReseniaResumenDto
        {
            PrendaId = request.PrendaId,
            Resenias = items.Select(x => new ReseniaResponseDto
            {
                Id = x.Id,
                PrendaId = x.PrendaId,
                UsuarioId = x.UsuarioId,
                Calificacion = x.Calificacion,
                Comentario = x.Comentario,
                Estado = x.Estado,
                ModeradoPorId = x.ModeradoPorId,
                ModeradoEn = x.ModeradoEn,
                MotivoRechazo = x.MotivoRechazo,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }),
            PromedioCalificacion = promedio,
            TotalResenias = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasNext = page < totalPages,
            HasPrev = page > 1
        });
    }
}
