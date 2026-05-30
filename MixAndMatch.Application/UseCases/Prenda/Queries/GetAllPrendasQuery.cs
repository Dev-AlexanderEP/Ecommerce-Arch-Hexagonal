using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetAllPrendasQuery : IRequest<ApiResponseDto<IEnumerable<PrendaResponseDto>>>
{
}

public class GetAllPrendasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllPrendasQuery, ApiResponseDto<IEnumerable<PrendaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<PrendaResponseDto>>> Handle(GetAllPrendasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<PrendaEntity>().GetAll();
        return ApiResponseDto<IEnumerable<PrendaResponseDto>>.Ok(items.Select(x => new PrendaResponseDto
        {
            Id = x.Id,
            Nombre = x.Nombre,
            Descripcion = x.Descripcion,
            MarcaId = x.MarcaId,
            CategoriaId = x.CategoriaId,
            ProveedorId = x.ProveedorId,
            GeneroId = x.GeneroId,
            Precio = x.Precio,
            Activo = x.Activo,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
