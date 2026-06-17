using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class CreateReseniaCommand : IRequest<ApiResponse<ReseniaResponseDto>>
{
    public required long PrendaId { get; set; }
    public required int Calificacion { get; set; }
    public string? Comentario { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }
}

public class CreateReseniaCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateReseniaCommand, ApiResponse<ReseniaResponseDto>>
{
    public async Task<ApiResponse<ReseniaResponseDto>> Handle(CreateReseniaCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Prendas.GetById(request.PrendaId) is null)
        {
            return ApiResponse<ReseniaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.", ErrorType.Validation);
        }

        var existing = await _uow.Resenias.GetByPrendaAndUsuarioAsync(request.PrendaId, request.SolicitanteId);
        if (existing is not null)
        {
            return ApiResponse<ReseniaResponseDto>.Fail("Ya tienes una resenia para esta prenda.", ErrorType.Conflict);
        }

        var entity = new ReseniaEntity
        {
            PrendaId = request.PrendaId,
            UsuarioId = request.SolicitanteId,
            Calificacion = request.Calificacion,
            Comentario = request.Comentario,
            Estado = EstadoResenia.PENDIENTE,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.Resenias.Add(entity);
        await _uow.Complete();

        return ApiResponse<ReseniaResponseDto>.Created(MapToDto(entity));
    }

    private static ReseniaResponseDto MapToDto(ReseniaEntity entity) => new()
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
    };
}
