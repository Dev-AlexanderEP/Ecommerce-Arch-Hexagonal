using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class DeletePagoCommand : IRequest<ApiResponseDto<bool>>
{
    public long Id { get; set; }
}

public class DeletePagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeletePagoCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(
        DeletePagoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<PagoEntity>()
                .GetById(request.Id);

            if (entity is null)
                return ApiResponseDto<bool>
                    .Fail($"Pago no encontrado para id {request.Id}");

            await _uow.Repository<PagoEntity>()
                .Delete(request.Id);

            await _uow.Complete();

            return ApiResponseDto<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Fail(ex.Message);
        }
    }
}