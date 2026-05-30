using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;

namespace MixAndMatch.Application.UseCases.Marca.Queries;

public class GetAllMarcasQuery : IRequest<ApiResponseDto<IEnumerable<MarcaResponseDto>>>
{
}

public class GetAllMarcasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllMarcasQuery, ApiResponseDto<IEnumerable<MarcaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<MarcaResponseDto>>> Handle(GetAllMarcasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<MarcaEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<MarcaResponseDto>>.Fail("No se encontraron marcas.");
        }

        return ApiResponseDto<IEnumerable<MarcaResponseDto>>.Ok(items.Select(x => new MarcaResponseDto
        {
            Id = x.Id,
            NomMarca = x.NomMarca
        }));
    }
}
