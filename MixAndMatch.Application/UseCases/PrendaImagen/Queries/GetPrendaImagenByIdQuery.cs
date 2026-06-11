using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Queries;

public class GetPrendaImagenByIdQuery : IRequest<ApiResponse<PrendaImagenResponseDto>>
{
    public required long PrendaImagenId { get; set; }
}

public class GetPrendaImagenByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetPrendaImagenByIdQuery, ApiResponse<PrendaImagenResponseDto>>
{
    public async Task<ApiResponse<PrendaImagenResponseDto>> Handle(GetPrendaImagenByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<PrendaImagenEntity>().GetById(request.PrendaImagenId);
        if (entity is null)
            return ApiResponse<PrendaImagenResponseDto>.Fail($"Imagen de prenda no encontrada para id {request.PrendaImagenId}.");

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
