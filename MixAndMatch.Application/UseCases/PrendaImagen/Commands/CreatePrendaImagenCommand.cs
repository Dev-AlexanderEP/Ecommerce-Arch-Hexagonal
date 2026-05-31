using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Commands;

public class CreatePrendaImagenCommand : IRequest<ApiResponseDto<PrendaImagenResponseDto>>
{
    public required long PrendaId { get; set; }
    public required string Tipo { get; set; }
    public required string Url { get; set; }
    public int? Orden { get; set; }
}

public class CreatePrendaImagenCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreatePrendaImagenCommand, ApiResponseDto<PrendaImagenResponseDto>>
{
    private static readonly HashSet<string> TiposValidos = ["principal", "hover", "galeria", "video"];

    public async Task<ApiResponseDto<PrendaImagenResponseDto>> Handle(CreatePrendaImagenCommand request, CancellationToken cancellationToken)
    {
        var prenda = await _uow.Repository<PrendaEntity>().GetById(request.PrendaId);
        if (prenda is null)
            return ApiResponseDto<PrendaImagenResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");

        var tipo = (request.Tipo ?? string.Empty).Trim().ToLowerInvariant();
        if (!TiposValidos.Contains(tipo))
            return ApiResponseDto<PrendaImagenResponseDto>.Fail("El tipo de imagen no es válido. Valores permitidos: principal, hover, galeria, video.");

        var url = (request.Url ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(url))
            return ApiResponseDto<PrendaImagenResponseDto>.Fail("La URL de la imagen es obligatoria.");

        var entity = new PrendaImagenEntity
        {
            PrendaId = request.PrendaId,
            Tipo = tipo,
            Url = url,
            Orden = request.Orden,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<PrendaImagenEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponseDto<PrendaImagenResponseDto>.Ok(ToDto(entity));
    }

    private static PrendaImagenResponseDto ToDto(PrendaImagenEntity e) => new()
    {
        Id = e.Id,
        PrendaId = e.PrendaId,
        Tipo = e.Tipo,
        Url = e.Url,
        Orden = e.Orden,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
