using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Carrito.Commands;

public class CreateCarritoCommand : IRequest<ApiResponse<CarritoResponseDto>>
{
    public required long UsuarioId { get; set; }
}

public class CreateCarritoCommandHandler(ICarritoRepository _carritos, IUnitOfWork _uow) : IRequestHandler<CreateCarritoCommand, ApiResponse<CarritoResponseDto>>
{
    public async Task<ApiResponse<CarritoResponseDto>> Handle(CreateCarritoCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Repository<UsuarioEntity>().GetById(request.UsuarioId);
        if (usuario is null)
        {
            return ApiResponse<CarritoResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        if (await _carritos.TieneCarritoActivo(request.UsuarioId))
        {
            return ApiResponse<CarritoResponseDto>.Fail("El usuario ya tiene un carrito activo.", ErrorType.Conflict);
        }

        var entity = new CarritoEntity
        {
            UsuarioId = request.UsuarioId,
            Estado = EstadoCarrito.ACTIVO,
            FechaCreacion = DateTime.UtcNow
        };

        await _carritos.Add(entity);
        await _uow.Complete();

        return ApiResponse<CarritoResponseDto>.Created(new CarritoResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado?.ToString(),
            UpdatedAt = entity.UpdatedAt
        });
    }
}
