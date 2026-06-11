using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class UpdateReseniaCommand : IRequest<ApiResponse<ReseniaResponseDto>>
{
    public required long ReseniaId { get; set; }

    public required int Calificacion { get; set; }

    public string? Comentario { get; set; }
}

public class UpdateReseniaCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdateReseniaCommand, ApiResponse<ReseniaResponseDto>>
{
    public async Task<ApiResponse<ReseniaResponseDto>> Handle(UpdateReseniaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ReseniaEntity>().GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponse<ReseniaResponseDto>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        if (entity.Estado != EstadoResenia.PENDIENTE)
        {
            return ApiResponse<ReseniaResponseDto>.Fail("Solo se pueden actualizar resenias en estado PENDIENTE.");
        }

        if (request.Calificacion < 1 || request.Calificacion > 5)
        {
            return ApiResponse<ReseniaResponseDto>.Fail("La calificacion debe estar entre 1 y 5.");
        }

        entity.Calificacion = request.Calificacion;
        entity.Comentario = request.Comentario;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Repository<ReseniaEntity>().Update(entity);
        await _uow.Complete();

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
