using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Enums;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.ValueObjects;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class UpdateReseniaCommand : IRequest<ApiResponseDto<ReseniaResponseDto>>
{
    public required long ReseniaId { get; set; }

    public required int Calificacion { get; set; }

    public string? Comentario { get; set; }
}

public class UpdateReseniaCommandHandler(IReseniaRepository _reseniaRepository, IUnitOfWork _uow)
    : IRequestHandler<UpdateReseniaCommand, ApiResponseDto<ReseniaResponseDto>>
{
    public async Task<ApiResponseDto<ReseniaResponseDto>> Handle(UpdateReseniaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ReseniaEntity>().GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        if (entity.Estado != EstadoResenia.PENDIENTE)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail("Solo se pueden actualizar resenias en estado PENDIENTE.");
        }

        if (request.Calificacion < 1 || request.Calificacion > 5)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail("La calificacion debe estar entre 1 y 5.");
        }

        entity.Calificacion = new Calificacion(request.Calificacion);
        entity.Comentario = request.Comentario;
        entity.UpdatedAt = DateTime.UtcNow;

        await _reseniaRepository.UpdateAsync(entity);
        await _uow.Complete();

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
