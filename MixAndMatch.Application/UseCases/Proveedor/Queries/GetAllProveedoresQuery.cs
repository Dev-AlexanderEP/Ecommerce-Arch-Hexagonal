using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

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
        var (items, total) = await _uow.Proveedores.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<ProveedorResponseDto>.Ok(items.Select(x => new ProveedorResponseDto
        {
            Id = x.Id,
            NomProveedor = x.NomProveedor
        }), total, request.Page, request.PageSize);
    }
}
