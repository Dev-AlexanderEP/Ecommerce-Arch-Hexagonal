using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Commands;

public class DeleteDescuentoUsuarioCommand : IRequest<ApiResponse<bool>>
{
    public required long DescuentoUsuarioId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class DeleteDescuentoUsuarioCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoUsuarioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDescuentoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.DescuentoUsuarios.GetById(request.DescuentoUsuarioId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Registro de uso de descuento no encontrado para id {request.DescuentoUsuarioId}.");
        }

        if (entity.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<bool>.Fail("No tienes acceso a este registro de uso de descuento.", ErrorType.Forbidden);
        }

        await _uow.DescuentoUsuarios.Delete(request.DescuentoUsuarioId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Registro de uso de descuento eliminado correctamente.");
    }
}
