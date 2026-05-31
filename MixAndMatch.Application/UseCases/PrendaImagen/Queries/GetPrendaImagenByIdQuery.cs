using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Queries;

public class GetPrendaImagenByIdQuery : IRequest<ApiResponseDto<PrendaImagenResponseDto>>
{
    public required long PrendaImagenId { get; set; }
}

public class GetPrendaImagenByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetPrendaImagenByIdQuery, ApiResponseDto<PrendaImagenResponseDto>>
{
    public async Task<ApiResponseDto<PrendaImagenResponseDto>> Handle(GetPrendaImagenByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<PrendaImagenEntity>().GetById(request.PrendaImagenId);
        if (entity is null)
            return ApiResponseDto<PrendaImagenResponseDto>.Fail($"Imagen de prenda no encontrada para id {request.PrendaImagenId}.");

        return ApiResponseDto<PrendaImagenResponseDto>.Ok(new PrendaImagenResponseDto
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
