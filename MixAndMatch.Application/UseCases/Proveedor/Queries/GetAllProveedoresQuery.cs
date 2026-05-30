using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Queries;

public class GetAllProveedoresQuery : IRequest<ApiResponseDto<IEnumerable<ProveedorResponseDto>>>
{
}

public class GetAllProveedoresQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllProveedoresQuery, ApiResponseDto<IEnumerable<ProveedorResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<ProveedorResponseDto>>> Handle(GetAllProveedoresQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<ProveedorEntity>().GetAll();
        return ApiResponseDto<IEnumerable<ProveedorResponseDto>>.Ok(items.Select(x => new ProveedorResponseDto
        {
            Id = x.Id,
            NomProveedor = x.NomProveedor
        }));
    }
}
