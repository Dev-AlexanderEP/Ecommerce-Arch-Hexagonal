using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Queries;

public class GetProveedorByIdQuery : IRequest<ApiResponse<ProveedorResponseDto>>
{
    public required long ProveedorId { get; set; }
}

public class GetProveedorByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetProveedorByIdQuery, ApiResponse<ProveedorResponseDto>>
{
    public async Task<ApiResponse<ProveedorResponseDto>> Handle(GetProveedorByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ProveedorEntity>().GetById(request.ProveedorId);
        if (entity is null)
        {
            return ApiResponse<ProveedorResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        }

        return ApiResponse<ProveedorResponseDto>.Ok(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
