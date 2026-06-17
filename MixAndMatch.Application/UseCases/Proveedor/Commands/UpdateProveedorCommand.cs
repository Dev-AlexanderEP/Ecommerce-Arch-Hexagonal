using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Proveedor.Commands;

public class UpdateProveedorCommand : IRequest<ApiResponse<ProveedorResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long ProveedorId { get; set; }
    public required string NomProveedor { get; set; }
}

public class UpdateProveedorCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateProveedorCommand, ApiResponse<ProveedorResponseDto>>
{
    public async Task<ApiResponse<ProveedorResponseDto>> Handle(UpdateProveedorCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Proveedores.GetById(request.ProveedorId);
        if (entity is null)
        {
            return ApiResponse<ProveedorResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        }

        var nombre = request.NomProveedor.Trim();
        if (await _uow.Proveedores.ExisteConNombre(nombre, request.ProveedorId))
        {
            return ApiResponse<ProveedorResponseDto>.Fail("El proveedor ya existe.", ErrorType.Conflict);
        }

        entity.NomProveedor = nombre;
        await _uow.Proveedores.Update(entity);
        await _uow.Complete();

        return ApiResponse<ProveedorResponseDto>.Ok(new ProveedorResponseDto
        {
            Id = entity.Id,
            NomProveedor = entity.NomProveedor
        });
    }
}
