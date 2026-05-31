using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Commands;

public class DeleteMetodoPagoCommand : IRequest<ApiResponseDto<bool>>
{
    public long Id { get; set; }
}

public class DeleteMetodoPagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteMetodoPagoCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(
        DeleteMetodoPagoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<Domain.Entities.MetodoPago>()
                .GetById(request.Id);

            if (entity is null)
            {
                return ApiResponseDto<bool>
                    .Fail($"Método de pago no encontrado para id {request.Id}");
            }

            await _uow.Repository<Domain.Entities.MetodoPago>()
                .Delete(request.Id);

            await _uow.Complete();

            return ApiResponseDto<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>
                .Fail(ex.Message);
        }
    }
}