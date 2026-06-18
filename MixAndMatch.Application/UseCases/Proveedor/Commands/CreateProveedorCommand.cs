using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Commands;

public class CreateProveedorCommand : IRequest<ApiResponse<ProveedorResponseDto>>
{
    public required string NomProveedor { get; set; }
}

public class CreateProveedorCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateProveedorCommand, ApiResponse<ProveedorResponseDto>>
{
    public async Task<ApiResponse<ProveedorResponseDto>> Handle(CreateProveedorCommand request, CancellationToken cancellationToken)
    {
        var nombre = request.NomProveedor.Trim();

        if (await _uow.Proveedores.ExisteConNombre(nombre))
        {
            return ApiResponse<ProveedorResponseDto>.Fail("El proveedor ya existe.", ErrorType.Conflict);
        }

        var entity = new ProveedorEntity { NomProveedor = nombre };
        await _uow.Proveedores.Add(entity);
        await _uow.Complete();

        return ApiResponse<ProveedorResponseDto>.Created(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
