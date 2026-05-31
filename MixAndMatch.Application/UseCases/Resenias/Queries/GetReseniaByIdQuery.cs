using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports.IRepositories;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetReseniaByIdQuery : IRequest<ApiResponseDto<ReseniaResponseDto>>
{
    public required long ReseniaId { get; set; }
}

public class GetReseniaByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetReseniaByIdQuery, ApiResponseDto<ReseniaResponseDto>>
{
    public async Task<ApiResponseDto<ReseniaResponseDto>> Handle(GetReseniaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ReseniaEntity>().GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        return ApiResponseDto<ReseniaResponseDto>.Ok(new ReseniaResponseDto
        {
            Id = entity.Id,
            PrendaId = entity.PrendaId,
            UsuarioId = entity.UsuarioId,
            Calificacion = entity.Calificacion.Valor,
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
