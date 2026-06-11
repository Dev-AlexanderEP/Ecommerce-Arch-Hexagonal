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
        var nombre = (request.NomProveedor ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<ProveedorResponseDto>.Fail("El nombre del proveedor es obligatorio.");
        }

        var repo = _uow.Repository<ProveedorEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomProveedor == nombre))
        {
            return ApiResponse<ProveedorResponseDto>.Fail("El proveedor ya existe.");
        }

        var entity = new ProveedorEntity { NomProveedor = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponse<ProveedorResponseDto>.Ok(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
