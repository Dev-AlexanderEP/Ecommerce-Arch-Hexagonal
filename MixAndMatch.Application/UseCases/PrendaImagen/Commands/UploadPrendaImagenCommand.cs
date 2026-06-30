using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Commands;

public class UploadPrendaImagenCommand : IRequest<ApiResponse<PrendaImagenResponseDto>>
{
    [JsonIgnore] public long PrendaId { get; set; }
    [JsonIgnore] public Stream Contenido { get; set; } = null!;
    [JsonIgnore] public string NombreArchivo { get; set; } = null!;
    [JsonIgnore] public string ContentType { get; set; } = null!;
    public required string Tipo { get; set; }
    public int? Orden { get; set; }
}

public class UploadPrendaImagenCommandHandler(IUnitOfWork _uow, IStorageService _storage)
    : IRequestHandler<UploadPrendaImagenCommand, ApiResponse<PrendaImagenResponseDto>>
{
    public async Task<ApiResponse<PrendaImagenResponseDto>> Handle(
        UploadPrendaImagenCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Prendas.GetById(request.PrendaId) is null)
            return ApiResponse<PrendaImagenResponseDto>.Fail(
                $"Prenda no encontrada para id {request.PrendaId}.", ErrorType.NotFound);

        if (!Enum.TryParse<TipoImagen>(request.Tipo, ignoreCase: true, out var tipo))
            return ApiResponse<PrendaImagenResponseDto>.Fail(
                $"Tipo inválido. Valores válidos: {string.Join(", ", Enum.GetNames<TipoImagen>())}.", ErrorType.Validation);

        var ext = Path.GetExtension(request.NombreArchivo).ToLowerInvariant();
        var fileKey = $"prendas/{request.PrendaId}/{Guid.NewGuid()}{ext}";

        var url = await _storage.UploadAsync(fileKey, request.Contenido, request.ContentType);

        var entity = new PrendaImagenEntity
        {
            PrendaId  = request.PrendaId,
            Tipo      = tipo,
            Url       = url,
            Orden     = request.Orden,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<PrendaImagenEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponse<PrendaImagenResponseDto>.Created(new PrendaImagenResponseDto
        {
            Id        = entity.Id,
            PrendaId  = entity.PrendaId,
            Tipo      = entity.Tipo.ToString(),
            Url       = entity.Url,
            Orden     = entity.Orden,
            CreatedAt = entity.CreatedAt
        });
    }
}
