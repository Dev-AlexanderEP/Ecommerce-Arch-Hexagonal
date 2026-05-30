using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Commands;

public class UpdateProveedorCommand : IRequest<ApiResponseDto<ProveedorResponseDto>>
{
    public required long ProveedorId { get; set; }
    public required string NomProveedor { get; set; }
}

public class UpdateProveedorCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateProveedorCommand, ApiResponseDto<ProveedorResponseDto>>
{
    public async Task<ApiResponseDto<ProveedorResponseDto>> Handle(UpdateProveedorCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<ProveedorEntity>();
        var entity = await repo.GetById(request.ProveedorId);
        if (entity is null)
        {
            return ApiResponseDto<ProveedorResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        }

        var nombre = (request.NomProveedor ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponseDto<ProveedorResponseDto>.Fail("El nombre del proveedor es obligatorio.");
        }

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.ProveedorId && x.NomProveedor == nombre))
        {
            return ApiResponseDto<ProveedorResponseDto>.Fail("El proveedor ya existe.");
        }

        entity.NomProveedor = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponseDto<ProveedorResponseDto>.Ok(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
