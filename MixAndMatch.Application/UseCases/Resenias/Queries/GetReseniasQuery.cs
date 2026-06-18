using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniasQuery : IRequest<ApiResponse<object>>
{
    public long? PrendaId { get; set; }
    public long? UsuarioId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReseniasQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetReseniasQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetReseniasQuery request, CancellationToken cancellationToken)
    {
        // Regla de entrada: exactamente uno de prendaId / usuarioId.
        if (request.PrendaId.HasValue == request.UsuarioId.HasValue)
        {
            return ApiResponse<object>.Fail("Debe enviar prendaId o usuarioId (solo uno).", ErrorType.Validation);
        }

        if (request.PrendaId.HasValue)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var (items, totalCount) = await _uow.Resenias.GetPaginatedByPrendaIdAsync(request.PrendaId.Value, page, pageSize);
            var promedio = await _uow.Resenias.GetPromedioByPrendaIdAsync(request.PrendaId.Value);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Una prenda sin resenias no es un error: 200 con lista vacia y promedio 0.
            return ApiResponse<object>.Ok(new ReseniaResumenDto
            {
                PrendaId = request.PrendaId.Value,
                Resenias = items.Select(MapToDto),
                PromedioCalificacion = promedio,
                TotalResenias = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNext = page < totalPages,
                HasPrev = page > 1
            });
        }

        var itemsByUsuario = await _uow.Resenias.GetByUsuarioIdAsync(request.UsuarioId!.Value);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiResponse<object>.Ok(itemsByUsuario.Select(MapToDto));
    }

    private static ReseniaResponseDto MapToDto(Domain.Entities.Resenia x) => new()
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
    };
}
