using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniasByUsuarioQuery : IRequest<ApiResponse<IEnumerable<ReseniaResponseDto>>>
{
    public required long UsuarioId { get; set; }
}

public class GetReseniasByUsuarioQueryHandler(IReseniaRepository _reseniaRepository)
    : IRequestHandler<GetReseniasByUsuarioQuery, ApiResponse<IEnumerable<ReseniaResponseDto>>>
{
    public async Task<ApiResponse<IEnumerable<ReseniaResponseDto>>> Handle(GetReseniasByUsuarioQuery request, CancellationToken cancellationToken)
    {
        var items = await _reseniaRepository.GetByUsuarioIdAsync(request.UsuarioId);
        if (!items.Any())
        {
            return ApiResponse<IEnumerable<ReseniaResponseDto>>.Fail("No se encontraron resenias para el usuario.");
        }

        return ApiResponse<IEnumerable<ReseniaResponseDto>>.Ok(items.Select(x => new ReseniaResponseDto
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
