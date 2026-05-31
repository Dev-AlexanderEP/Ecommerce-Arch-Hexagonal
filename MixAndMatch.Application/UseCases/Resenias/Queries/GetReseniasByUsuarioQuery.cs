using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniasByUsuarioQuery : IRequest<ApiResponseDto<IEnumerable<ReseniaResponseDto>>>
{
    public required long UsuarioId { get; set; }
}

public class GetReseniasByUsuarioQueryHandler(IReseniaRepository _reseniaRepository)
    : IRequestHandler<GetReseniasByUsuarioQuery, ApiResponseDto<IEnumerable<ReseniaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<ReseniaResponseDto>>> Handle(GetReseniasByUsuarioQuery request, CancellationToken cancellationToken)
    {
        var items = await _reseniaRepository.GetByUsuarioIdAsync(request.UsuarioId);
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<ReseniaResponseDto>>.Fail("No se encontraron resenias para el usuario.");
        }

        return ApiResponseDto<IEnumerable<ReseniaResponseDto>>.Ok(items.Select(x => new ReseniaResponseDto
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
