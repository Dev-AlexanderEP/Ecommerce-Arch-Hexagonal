using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Queries;

public class GetAllPrendaImagenesQuery : IRequest<ApiResponseDto<IEnumerable<PrendaImagenResponseDto>>> { }

public class GetAllPrendaImagenesQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllPrendaImagenesQuery, ApiResponseDto<IEnumerable<PrendaImagenResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<PrendaImagenResponseDto>>> Handle(GetAllPrendaImagenesQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<PrendaImagenEntity>().GetAll();
        if (!items.Any())
            return ApiResponseDto<IEnumerable<PrendaImagenResponseDto>>.Fail("No se encontraron imágenes de prendas.");

        return ApiResponseDto<IEnumerable<PrendaImagenResponseDto>>.Ok(items.Select(x => new PrendaImagenResponseDto
        {
            Id = x.Id,
            PrendaId = x.PrendaId,
            Tipo = x.Tipo,
            Url = x.Url,
            Orden = x.Orden,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
