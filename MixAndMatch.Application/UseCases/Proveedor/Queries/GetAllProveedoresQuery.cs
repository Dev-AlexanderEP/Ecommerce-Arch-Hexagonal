using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Queries;

public class GetAllProveedoresQuery : IRequest<ApiPaginationResponse<ProveedorResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllProveedoresQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllProveedoresQuery, ApiPaginationResponse<ProveedorResponseDto>>
{
    public async Task<ApiPaginationResponse<ProveedorResponseDto>> Handle(GetAllProveedoresQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<ProveedorEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<ProveedorResponseDto>.Fail("No se encontraron proveedores.");
        }

        return ApiPaginationResponse<ProveedorResponseDto>.Ok(items.Select(x => new ProveedorResponseDto
        {
            Id = x.Id,
            NomProveedor = x.NomProveedor
        }), total, request.Page, request.PageSize);
    }
}
