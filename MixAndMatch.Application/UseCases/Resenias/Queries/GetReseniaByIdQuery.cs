using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniaByIdQuery : IRequest<ApiResponse<ReseniaResponseDto>>
{
    public required long ReseniaId { get; set; }
}

public class GetReseniaByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetReseniaByIdQuery, ApiResponse<ReseniaResponseDto>>
{
    public async Task<ApiResponse<ReseniaResponseDto>> Handle(GetReseniaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Resenias.GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponse<ReseniaResponseDto>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        return ApiResponse<ReseniaResponseDto>.Ok(new ReseniaResponseDto
        {
            Id = entity.Id,
            PrendaId = entity.PrendaId,
            UsuarioId = entity.UsuarioId,
            Calificacion = entity.Calificacion,
            Comentario = entity.Comentario,
            Estado = entity.Estado,
            ModeradoPorId = entity.ModeradoPorId,
            ModeradoEn = entity.ModeradoEn,
            MotivoRechazo = entity.MotivoRechazo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
