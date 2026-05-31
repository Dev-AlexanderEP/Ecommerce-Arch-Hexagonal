using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Enums;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Domain.Ports.IRepositories;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class UpdateEstadoReseniaCommand : IRequest<ApiResponseDto<ReseniaResponseDto>>
{
    public required long ReseniaId { get; set; }

    public required EstadoResenia Estado { get; set; }

    public Guid ModeradoPorId { get; set; }

    public string? MotivoRechazo { get; set; }
}

public class UpdateEstadoReseniaCommandHandler(IReseniaRepository _reseniaRepository, IUnitOfWork _uow)
    : IRequestHandler<UpdateEstadoReseniaCommand, ApiResponseDto<ReseniaResponseDto>>
{
    public async Task<ApiResponseDto<ReseniaResponseDto>> Handle(UpdateEstadoReseniaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ReseniaEntity>().GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        if (request.Estado == EstadoResenia.PENDIENTE)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail("El estado de moderacion no puede ser PENDIENTE.");
        }

        if (request.Estado == EstadoResenia.RECHAZADA && string.IsNullOrWhiteSpace(request.MotivoRechazo))
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail("El motivo de rechazo es obligatorio.");
        }

        entity.Estado = request.Estado;
        entity.ModeradoPorId = request.ModeradoPorId == Guid.Empty ? null : request.ModeradoPorId;
        entity.ModeradoEn = DateTime.UtcNow;
        entity.MotivoRechazo = request.Estado == EstadoResenia.RECHAZADA
            ? request.MotivoRechazo?.Trim()
            : null;
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
