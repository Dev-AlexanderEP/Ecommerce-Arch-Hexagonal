using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Proveedor.Commands;

public class DeleteProveedorCommand : IRequest<ApiResponse<bool>>
{
    public required long ProveedorId { get; set; }
}

public class DeleteProveedorCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteProveedorCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteProveedorCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Proveedores.GetById(request.ProveedorId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        }

        if (await _uow.Proveedores.TienePrendas(request.ProveedorId))
        {
            return ApiResponse<bool>.Fail("El proveedor tiene prendas asociadas.", ErrorType.Conflict);
        }

        await _uow.Proveedores.Delete(request.ProveedorId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Proveedor eliminado correctamente.");
    }
}
