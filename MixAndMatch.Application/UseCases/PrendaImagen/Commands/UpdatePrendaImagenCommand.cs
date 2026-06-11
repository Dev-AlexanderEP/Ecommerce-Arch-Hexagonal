using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Commands;

public class UpdatePrendaImagenCommand : IRequest<ApiResponse<PrendaImagenResponseDto>>
{
    public required long PrendaImagenId { get; set; }
    public required string Tipo { get; set; }
    public required string Url { get; set; }
    public int? Orden { get; set; }
}

public class UpdatePrendaImagenCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdatePrendaImagenCommand, ApiResponse<PrendaImagenResponseDto>>
{
    private static readonly HashSet<string> TiposValidos = ["principal", "hover", "galeria", "video"];

    public async Task<ApiResponse<PrendaImagenResponseDto>> Handle(UpdatePrendaImagenCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaImagenEntity>();
        var entity = await repo.GetById(request.PrendaImagenId);
        if (entity is null)
            return ApiResponse<PrendaImagenResponseDto>.Fail($"Imagen de prenda no encontrada para id {request.PrendaImagenId}.");

        var tipo = (request.Tipo ?? string.Empty).Trim().ToLowerInvariant();
        if (!TiposValidos.Contains(tipo))
            return ApiResponse<PrendaImagenResponseDto>.Fail("El tipo de imagen no es vÃ¡lido. Valores permitidos: principal, hover, galeria, video.");

        var url = (request.Url ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(url))
            return ApiResponse<PrendaImagenResponseDto>.Fail("La URL de la imagen es obligatoria.");

        entity.Tipo = tipo;
        entity.Url = url;
        entity.Orden = request.Orden;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<PrendaImagenResponseDto>.Ok(new PrendaImagenResponseDto
        {
            Id = entity.Id,
            PrendaId = entity.PrendaId,
            Tipo = entity.Tipo,
            Url = entity.Url,
            Orden = entity.Orden,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
