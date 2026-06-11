using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniasByPrendaQuery : IRequest<ApiResponse<ReseniaResumenDto>>
{
    public required long PrendaId { get; set; }

    public required int Page { get; set; }

    public required int PageSize { get; set; }
}

public class GetReseniasByPrendaQueryHandler(IReseniaRepository _reseniaRepository)
    : IRequestHandler<GetReseniasByPrendaQuery, ApiResponse<ReseniaResumenDto>>
{
    public async Task<ApiResponse<ReseniaResumenDto>> Handle(GetReseniasByPrendaQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
        {
            return ApiResponse<ReseniaResumenDto>.Fail("Page y pageSize deben ser mayores a 0.");
        }

        var (items, totalCount) = await _reseniaRepository.GetPaginatedByPrendaIdAsync(
            request.PrendaId,
            request.Page,
            request.PageSize);

        if (totalCount == 0)
        {
            return ApiResponse<ReseniaResumenDto>.Fail("No se encontraron resenias para la prenda.");
        }

        var promedio = await _reseniaRepository.GetPromedioByPrendaIdAsync(request.PrendaId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var resumen = new ReseniaResumenDto
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
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasNext = request.Page < totalPages,
            HasPrev = request.Page > 1
        };

        return ApiResponse<ReseniaResumenDto>.Ok(resumen);
    }
}
