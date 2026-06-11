using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;

namespace MixAndMatch.Application.UseCases.Carrito.Commands;

public class UpdateCarritoCommand : IRequest<ApiResponse<CarritoResponseDto>>
{
    public required long CarritoId { get; set; }
    public required string Estado { get; set; }
}

public class UpdateCarritoCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateCarritoCommand, ApiResponse<CarritoResponseDto>>
{
    public async Task<ApiResponse<CarritoResponseDto>> Handle(UpdateCarritoCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CarritoEntity>();
        var entity = await repo.GetById(request.CarritoId);
        if (entity is null)
        {
            return ApiResponse<CarritoResponseDto>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        entity.Estado = request.Estado;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<CarritoResponseDto>.Ok(new CarritoResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
