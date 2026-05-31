using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Queries;

public class GetAllTallasQuery : IRequest<ApiResponseDto<IEnumerable<TallaResponseDto>>> { }

public class GetAllTallasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllTallasQuery, ApiResponseDto<IEnumerable<TallaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<TallaResponseDto>>> Handle(GetAllTallasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<TallaEntity>().GetAll();
        if (!items.Any())
            return ApiResponseDto<IEnumerable<TallaResponseDto>>.Fail("No se encontraron tallas.");

        return ApiResponseDto<IEnumerable<TallaResponseDto>>.Ok(
            items.Select(x => new TallaResponseDto { Id = x.Id, NomTalla = x.NomTalla }));
    }
}
