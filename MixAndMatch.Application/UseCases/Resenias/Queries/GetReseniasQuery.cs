using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniasQuery : IRequest<ApiResponse<object>>
{
    public long? PrendaId { get; set; }

    public long? UsuarioId { get; set; }

    public required int Page { get; set; }

    public required int PageSize { get; set; }
}

public class GetReseniasQueryHandler(IReseniaRepository _reseniaRepository)
    : IRequestHandler<GetReseniasQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetReseniasQuery request, CancellationToken cancellationToken)
    {
        if (request.PrendaId.HasValue == request.UsuarioId.HasValue)
        {
            return ApiResponse<object>.Fail("Debe enviar prendaId o usuarioId.");
        }

        if (request.PrendaId.HasValue)
        {
            if (request.Page <= 0 || request.PageSize <= 0)
            {
                return ApiResponse<object>.Fail("Page y pageSize deben ser mayores a 0.");
            }

            var (items, totalCount) = await _reseniaRepository.GetPaginatedByPrendaIdAsync(
                request.PrendaId.Value,
                request.Page,
                request.PageSize);

            if (totalCount == 0)
            {
                return ApiResponse<object>.Fail("No se encontraron resenias para la prenda.");
            }

            var promedio = await _reseniaRepository.GetPromedioByPrendaIdAsync(request.PrendaId.Value);
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var resumen = new ReseniaResumenDto
            {
                PrendaId = request.PrendaId.Value,
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

            return ApiResponse<object>.Ok(resumen);
        }

        var itemsByUsuario = await _reseniaRepository.GetByUsuarioIdAsync(request.UsuarioId!.Value);
        if (!itemsByUsuario.Any())
        {
            return ApiResponse<object>.Fail("No se encontraron resenias para el usuario.");
        }

        return ApiResponse<object>.Ok(itemsByUsuario.Select(x => new ReseniaResponseDto
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
        }));
    }
}
