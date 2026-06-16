using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;

namespace MixAndMatch.Application.UseCases.Carrito.Commands;

public class UpdateCarritoCommand : IRequest<ApiResponse<CarritoResponseDto>>
{
    public required long CarritoId { get; set; }
    public required string Estado { get; set; }
    public required long SolicitanteId { get; set; }
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

        if (entity.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<CarritoResponseDto>.Fail("No tienes acceso a este carrito.", ErrorType.Forbidden);
        }

        // El formato del estado ya lo valida UpdateCarritoCommandValidator (400); esto es defensa.
        if (!Enum.TryParse<EstadoCarrito>(request.Estado, ignoreCase: true, out var nuevoEstado))
        {
            return ApiResponse<CarritoResponseDto>.Fail($"Estado inválido: {request.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoCarrito>())}.", ErrorType.Validation);
        }

        entity.Estado = nuevoEstado;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<CarritoResponseDto>.Ok(new CarritoResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado?.ToString(),
            UpdatedAt = entity.UpdatedAt
        });
    }
}
