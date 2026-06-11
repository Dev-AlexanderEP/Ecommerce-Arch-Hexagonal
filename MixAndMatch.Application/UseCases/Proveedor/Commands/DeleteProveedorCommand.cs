using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Proveedor.Commands;

public class DeleteProveedorCommand : IRequest<ApiResponse<bool>>
{
    public required long ProveedorId { get; set; }
}

public class DeleteProveedorCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteProveedorCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteProveedorCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<ProveedorEntity>();
        var entity = await repo.GetById(request.ProveedorId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        }

        var prendas = await _uow.Repository<PrendaEntity>().GetAll();
        if (prendas.Any(x => x.ProveedorId == request.ProveedorId))
        {
            return ApiResponse<bool>.Fail("El proveedor tiene prendas asociadas.");
        }

        await repo.Delete(request.ProveedorId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Proveedor eliminado correctamente.");
    }
}
