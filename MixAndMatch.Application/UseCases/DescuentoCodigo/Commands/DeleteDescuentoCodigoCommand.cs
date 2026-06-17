using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class DeleteDescuentoCodigoCommand : IRequest<ApiResponse<bool>>
{
    public required long DescuentoCodigoId { get; set; }
}

public class DeleteDescuentoCodigoCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoCodigoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDescuentoCodigoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.DescuentoCodigos.GetById(request.DescuentoCodigoId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Descuento de código no encontrado para id {request.DescuentoCodigoId}.");
        }

        if (await _uow.DescuentoCodigos.TieneUsos(request.DescuentoCodigoId))
        {
            return ApiResponse<bool>.Fail("El código de descuento tiene usos asociados.", ErrorType.Conflict);
        }

        await _uow.DescuentoCodigos.Delete(request.DescuentoCodigoId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Descuento de código eliminado correctamente.");
    }
}
