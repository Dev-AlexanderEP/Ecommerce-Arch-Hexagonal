using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetAllPrendasQuery : IRequest<ApiPaginationResponse<PrendaResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllPrendasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllPrendasQuery, ApiPaginationResponse<PrendaResponseDto>>
{
    public async Task<ApiPaginationResponse<PrendaResponseDto>> Handle(GetAllPrendasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<PrendaEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<PrendaResponseDto>.Fail("No se encontraron prendas.");
        }

        return ApiPaginationResponse<PrendaResponseDto>.Ok(items.Select(x => new PrendaResponseDto
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
        }), total, request.Page, request.PageSize);
    }
}
