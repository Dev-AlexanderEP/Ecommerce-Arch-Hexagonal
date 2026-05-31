using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;

namespace MixAndMatch.Application.UseCases.Carrito.Commands;

public class UpdateCarritoCommand : IRequest<ApiResponseDto<CarritoResponseDto>>
{
    public required long CarritoId { get; set; }
    public required string Estado { get; set; }
}

public class UpdateCarritoCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateCarritoCommand, ApiResponseDto<CarritoResponseDto>>
{
    public async Task<ApiResponseDto<CarritoResponseDto>> Handle(UpdateCarritoCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CarritoEntity>();
        var entity = await repo.GetById(request.CarritoId);
        if (entity is null)
        {
            return ApiResponseDto<CarritoResponseDto>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        entity.Estado = request.Estado;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponseDto<CarritoResponseDto>.Ok(new CarritoResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
