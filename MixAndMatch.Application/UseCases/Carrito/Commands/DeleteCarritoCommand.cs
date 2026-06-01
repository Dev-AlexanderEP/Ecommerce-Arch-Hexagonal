using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;
using CarritoItemEntity = MixAndMatch.Domain.Entities.CarritoItem;

namespace MixAndMatch.Application.UseCases.Carrito.Commands;

public class DeleteCarritoCommand : IRequest<ApiResponseDto<bool>>
{
    public required long CarritoId { get; set; }
}

public class DeleteCarritoCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteCarritoCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteCarritoCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CarritoEntity>();
        var entity = await repo.GetById(request.CarritoId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        var items = await _uow.Repository<CarritoItemEntity>().GetAll();
        if (items.Any(x => x.CarritoId == request.CarritoId))
        {
            return ApiResponseDto<bool>.Fail("El carrito tiene ítems asociados y no puede eliminarse.");
        }

        await repo.Delete(request.CarritoId);
        await _uow.Complete();

        return ApiResponseDto<bool>.Ok(true, "Carrito eliminado correctamente.");
    }
}
