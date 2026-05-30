using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;

namespace MixAndMatch.Application.UseCases.Genero.Queries;

public class GetAllGenerosQuery : IRequest<ApiResponseDto<IEnumerable<GeneroResponseDto>>>
{
}

public class GetAllGenerosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllGenerosQuery, ApiResponseDto<IEnumerable<GeneroResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<GeneroResponseDto>>> Handle(GetAllGenerosQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<GeneroEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<GeneroResponseDto>>.Fail("No se encontraron géneros.");
        }

        return ApiResponseDto<IEnumerable<GeneroResponseDto>>.Ok(items.Select(x => new GeneroResponseDto
        {
            Id = x.Id,
            NomGenero = x.NomGenero
        }));
    }
}
