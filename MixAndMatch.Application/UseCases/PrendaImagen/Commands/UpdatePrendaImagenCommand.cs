using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Commands;

public class UpdatePrendaImagenCommand : IRequest<ApiResponse<PrendaImagenResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long PrendaImagenId { get; set; }
    public required string Tipo { get; set; }
    public required string Url { get; set; }
    public int? Orden { get; set; }
}

public class UpdatePrendaImagenCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdatePrendaImagenCommand, ApiResponse<PrendaImagenResponseDto>>
{
    public async Task<ApiResponse<PrendaImagenResponseDto>> Handle(UpdatePrendaImagenCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaImagenEntity>();
        var entity = await repo.GetById(request.PrendaImagenId);
        if (entity is null)
        {
            return ApiResponse<PrendaImagenResponseDto>.Fail($"Imagen de prenda no encontrada para id {request.PrendaImagenId}.");
        }

        // El formato del tipo ya lo valida UpdatePrendaImagenCommandValidator (400); esto es defensa.
        if (!Enum.TryParse<TipoImagen>(request.Tipo, ignoreCase: true, out var tipoImagen))
        {
            return ApiResponse<PrendaImagenResponseDto>.Fail($"Tipo de imagen inválido: {request.Tipo}. Permitidos: {string.Join(", ", Enum.GetNames<TipoImagen>())}.", ErrorType.Validation);
        }

        entity.Tipo = tipoImagen;
        entity.Url = request.Url.Trim();
        entity.Orden = request.Orden;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<PrendaImagenResponseDto>.Ok(new PrendaImagenResponseDto
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
