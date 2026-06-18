using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Commands;

public class CreatePrendaImagenCommand : IRequest<ApiResponse<PrendaImagenResponseDto>>
{
    public required long PrendaId { get; set; }
    public required string Tipo { get; set; }
    public required string Url { get; set; }
    public int? Orden { get; set; }
}

public class CreatePrendaImagenCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreatePrendaImagenCommand, ApiResponse<PrendaImagenResponseDto>>
{
    public async Task<ApiResponse<PrendaImagenResponseDto>> Handle(CreatePrendaImagenCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Prendas.GetById(request.PrendaId) is null)
        {
            return ApiResponse<PrendaImagenResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.", ErrorType.Validation);
        }

        // El formato del tipo ya lo valida CreatePrendaImagenCommandValidator (400); esto es defensa.
        if (!Enum.TryParse<TipoImagen>(request.Tipo, ignoreCase: true, out var tipoImagen))
        {
            return ApiResponse<PrendaImagenResponseDto>.Fail($"Tipo de imagen inválido: {request.Tipo}. Permitidos: {string.Join(", ", Enum.GetNames<TipoImagen>())}.", ErrorType.Validation);
        }

        var entity = new PrendaImagenEntity
        {
            PrendaId = request.PrendaId,
            Tipo = tipoImagen,
            Url = request.Url.Trim(),
            Orden = request.Orden,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<PrendaImagenEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponse<PrendaImagenResponseDto>.Created(new PrendaImagenResponseDto
        {
            Id = entity.Id,
            PrendaId = entity.PrendaId,
            Tipo = entity.Tipo.ToString(),
            Url = entity.Url,
            Orden = entity.Orden,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
