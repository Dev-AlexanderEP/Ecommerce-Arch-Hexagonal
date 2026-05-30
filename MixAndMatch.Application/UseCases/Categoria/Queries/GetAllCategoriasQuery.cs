using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Queries;

public class GetAllCategoriasQuery : IRequest<ApiResponseDto<IEnumerable<CategoriaResponseDto>>>
{
}

public class GetAllCategoriasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllCategoriasQuery, ApiResponseDto<IEnumerable<CategoriaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<CategoriaResponseDto>>> Handle(GetAllCategoriasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<CategoriaEntity>().GetAll();
        return ApiResponseDto<IEnumerable<CategoriaResponseDto>>.Ok(items.Select(x => new CategoriaResponseDto
        {
            Id = x.Id,
            NomCategoria = x.NomCategoria
        }));
    }
}
