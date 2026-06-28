using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class BuscarReseniasQuery : IRequest<ApiPaginationResponse<ReseniaResponseDto>>
{
    public long? PrendaId { get; set; }
    public long? UsuarioId { get; set; }
    public int? Calificacion { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class BuscarReseniasQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<BuscarReseniasQuery, ApiPaginationResponse<ReseniaResponseDto>>
{
    public async Task<ApiPaginationResponse<ReseniaResponseDto>> Handle(BuscarReseniasQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _uow.Resenias.BuscarAsync(
            request.PrendaId, request.UsuarioId, request.Calificacion, page, pageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<ReseniaResponseDto>.Ok(items.Select(x => new ReseniaResponseDto
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
        }), totalCount, page, pageSize);
    }
}
