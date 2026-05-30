using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Queries;

public class GetProveedorByIdQuery : IRequest<ApiResponseDto<ProveedorResponseDto>>
{
    public required long ProveedorId { get; set; }
}

public class GetProveedorByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetProveedorByIdQuery, ApiResponseDto<ProveedorResponseDto>>
{
    public async Task<ApiResponseDto<ProveedorResponseDto>> Handle(GetProveedorByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ProveedorEntity>().GetById(request.ProveedorId);
        if (entity is null)
        {
            return ApiResponseDto<ProveedorResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        }

        return ApiResponseDto<ProveedorResponseDto>.Ok(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
