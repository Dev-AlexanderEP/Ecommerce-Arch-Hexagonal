using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

namespace MixAndMatch.Application.UseCases.CarritoItem.Commands;

public class DeleteCarritoItemCommand : IRequest<ApiResponseDto<bool>>
{
    public required long CarritoItemId { get; set; }
}

public class DeleteCarritoItemCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteCarritoItemCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteCarritoItemCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CarritoItemEntity>();
        var entity = await repo.GetById(request.CarritoItemId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"CarritoItem no encontrado para id {request.CarritoItemId}.");
        }

        await repo.Delete(request.CarritoItemId);
        await _uow.Complete();

        return ApiResponseDto<bool>.Ok(true, "CarritoItem eliminado correctamente.");
    }
}
