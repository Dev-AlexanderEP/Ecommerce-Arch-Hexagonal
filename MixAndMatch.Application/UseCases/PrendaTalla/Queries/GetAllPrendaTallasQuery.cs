using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Queries;

public class GetAllPrendaTallasQuery : IRequest<ApiResponseDto<IEnumerable<PrendaTallaResponseDto>>> { }

public class GetAllPrendaTallasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllPrendaTallasQuery, ApiResponseDto<IEnumerable<PrendaTallaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<PrendaTallaResponseDto>>> Handle(GetAllPrendaTallasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<PrendaTallaEntity>().GetAll();
        if (!items.Any())
            return ApiResponseDto<IEnumerable<PrendaTallaResponseDto>>.Fail("No se encontraron combinaciones de prenda y talla.");

        return ApiResponseDto<IEnumerable<PrendaTallaResponseDto>>.Ok(items.Select(x => new PrendaTallaResponseDto
        {
            Id = x.Id,
            PrendaId = x.PrendaId,
            TallaId = x.TallaId,
            Stock = x.Stock,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
