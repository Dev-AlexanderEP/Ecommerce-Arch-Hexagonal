using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Commands;

public class UpdateProveedorCommand : IRequest<ApiResponse<ProveedorResponseDto>>
{
    public required long ProveedorId { get; set; }
    public required string NomProveedor { get; set; }
}

public class UpdateProveedorCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateProveedorCommand, ApiResponse<ProveedorResponseDto>>
{
    public async Task<ApiResponse<ProveedorResponseDto>> Handle(UpdateProveedorCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<ProveedorEntity>();
        var entity = await repo.GetById(request.ProveedorId);
        if (entity is null)
        {
            return ApiResponse<ProveedorResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        }

        var nombre = (request.NomProveedor ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<ProveedorResponseDto>.Fail("El nombre del proveedor es obligatorio.");
        }

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.ProveedorId && x.NomProveedor == nombre))
        {
            return ApiResponse<ProveedorResponseDto>.Fail("El proveedor ya existe.");
        }

        entity.NomProveedor = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<ProveedorResponseDto>.Ok(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
