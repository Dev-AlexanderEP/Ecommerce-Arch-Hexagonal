using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Commands;

public class CreateProveedorCommand : IRequest<ApiResponseDto<ProveedorResponseDto>>
{
    public required string NomProveedor { get; set; }
}

public class CreateProveedorCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateProveedorCommand, ApiResponseDto<ProveedorResponseDto>>
{
    public async Task<ApiResponseDto<ProveedorResponseDto>> Handle(CreateProveedorCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.NomProveedor ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponseDto<ProveedorResponseDto>.Fail("El nombre del proveedor es obligatorio.");
        }

        var repo = _uow.Repository<ProveedorEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomProveedor == nombre))
        {
            return ApiResponseDto<ProveedorResponseDto>.Fail("El proveedor ya existe.");
        }

        var entity = new ProveedorEntity { NomProveedor = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponseDto<ProveedorResponseDto>.Ok(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
