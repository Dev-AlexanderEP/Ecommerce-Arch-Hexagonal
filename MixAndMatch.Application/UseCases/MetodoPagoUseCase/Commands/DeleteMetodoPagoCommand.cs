using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Commands;

public class DeleteMetodoPagoCommand : IRequest<ApiResponse<bool>>
{
    public long Id { get; set; }
}

public class DeleteMetodoPagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteMetodoPagoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteMetodoPagoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<Domain.Entities.MetodoPago>()
                .GetById(request.Id);

            if (entity is null)
            {
                return ApiResponse<bool>
                    .Fail($"MÃ©todo de pago no encontrado para id {request.Id}");
            }

            await _uow.Repository<Domain.Entities.MetodoPago>()
                .Delete(request.Id);

            await _uow.Complete();

            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>
                .Fail(ex.Message);
        }
    }
}