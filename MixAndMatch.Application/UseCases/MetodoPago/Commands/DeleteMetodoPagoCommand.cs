using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Commands;

public class DeleteMetodoPagoCommand : IRequest<ApiResponse<bool>>
{
    public required long Id { get; set; }
}

public class DeleteMetodoPagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteMetodoPagoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteMetodoPagoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.MetodoPagos.GetById(request.Id);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Metodo de pago no encontrado para id {request.Id}.");
        }

        if (await _uow.MetodoPagos.TienePagos(request.Id))
        {
            return ApiResponse<bool>.Fail("El metodo de pago tiene pagos asociados.", ErrorType.Conflict);
        }

        await _uow.MetodoPagos.Delete(request.Id);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Metodo de pago eliminado correctamente.");
    }
}
