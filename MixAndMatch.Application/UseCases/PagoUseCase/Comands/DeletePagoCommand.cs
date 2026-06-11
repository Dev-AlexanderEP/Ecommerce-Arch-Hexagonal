using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class DeletePagoCommand : IRequest<ApiResponse<bool>>
{
    public long Id { get; set; }
}

public class DeletePagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeletePagoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeletePagoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _uow.Repository<PagoEntity>()
                .GetById(request.Id);

            if (entity is null)
                return ApiResponse<bool>
                    .Fail($"Pago no encontrado para id {request.Id}");

            await _uow.Repository<PagoEntity>()
                .Delete(request.Id);

            await _uow.Complete();

            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }
}