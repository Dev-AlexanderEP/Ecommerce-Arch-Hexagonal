using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.ValueObjects;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class CreateReseniaCommand : IRequest<ApiResponseDto<ReseniaResponseDto>>
{
    public required long PrendaId { get; set; }

    public required long UsuarioId { get; set; }

    public required int Calificacion { get; set; }

    public string? Comentario { get; set; }
}

public class CreateReseniaCommandHandler(IReseniaRepository _reseniaRepository, IUnitOfWork _uow)
    : IRequestHandler<CreateReseniaCommand, ApiResponseDto<ReseniaResponseDto>>
{
    public async Task<ApiResponseDto<ReseniaResponseDto>> Handle(CreateReseniaCommand request, CancellationToken cancellationToken)
    {
        if (request.Calificacion < 1 || request.Calificacion > 5)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail("La calificacion debe estar entre 1 y 5.");
        }

        var existing = await _reseniaRepository.GetByPrendaAndUsuarioAsync(request.PrendaId, request.UsuarioId);
        if (existing is not null)
        {
            return ApiResponseDto<ReseniaResponseDto>.Fail("El usuario ya tiene una resenia para esta prenda.");
        }

        var entity = new ReseniaEntity
        {
            PrendaId = request.PrendaId,
            UsuarioId = request.UsuarioId,
            Calificacion = new Calificacion(request.Calificacion),
            Comentario = request.Comentario,
            Estado = EstadoResenia.PENDIENTE,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _reseniaRepository.AddAsync(entity);
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
