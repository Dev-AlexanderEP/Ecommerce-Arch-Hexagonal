using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoUsuarioEntity = MixAndMatch.Domain.Entities.DescuentoUsuario;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Commands;

public class DeleteDescuentoUsuarioCommand : IRequest<ApiResponse<bool>>
{
    public required long DescuentoUsuarioId { get; set; }
}

public class DeleteDescuentoUsuarioCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoUsuarioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDescuentoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DescuentoUsuarioEntity>();
        var entity = await repo.GetById(request.DescuentoUsuarioId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Registro de uso de descuento no encontrado para id {request.DescuentoUsuarioId}.");
        }

        await repo.Delete(request.DescuentoUsuarioId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Registro de uso de descuento eliminado correctamente.");
    }
}
