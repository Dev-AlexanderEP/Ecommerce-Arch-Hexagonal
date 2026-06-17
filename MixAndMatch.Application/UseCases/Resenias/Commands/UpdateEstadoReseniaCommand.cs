using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class UpdateEstadoReseniaCommand : IRequest<ApiResponse<ReseniaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long ReseniaId { get; set; }
    public required EstadoResenia Estado { get; set; }
    public Guid ModeradoPorId { get; set; }
    public string? MotivoRechazo { get; set; }
}

public class UpdateEstadoReseniaCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdateEstadoReseniaCommand, ApiResponse<ReseniaResponseDto>>
{
    public async Task<ApiResponse<ReseniaResponseDto>> Handle(UpdateEstadoReseniaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Resenias.GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponse<ReseniaResponseDto>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        entity.Estado = request.Estado;
        entity.ModeradoPorId = request.ModeradoPorId == Guid.Empty ? null : request.ModeradoPorId;
        entity.ModeradoEn = DateTime.UtcNow;
        entity.MotivoRechazo = request.Estado == EstadoResenia.RECHAZADA
            ? request.MotivoRechazo?.Trim()
            : null;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Resenias.Update(entity);
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
