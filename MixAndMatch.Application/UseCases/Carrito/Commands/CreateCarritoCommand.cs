using MediatR;
using MixAndMatch.Application.Common;
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

public class CreateCarritoCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateCarritoCommand, ApiResponse<CarritoResponseDto>>
{
    public async Task<ApiResponse<CarritoResponseDto>> Handle(CreateCarritoCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Repository<UsuarioEntity>().GetById(request.UsuarioId);
        if (usuario is null)
        {
            return ApiResponse<CarritoResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        var carritos = await _uow.Repository<CarritoEntity>().GetAll();
        if (carritos.Any(x => x.UsuarioId == request.UsuarioId && x.Estado == "ACTIVO"))
        {
            return ApiResponse<CarritoResponseDto>.Fail("El usuario ya tiene un carrito activo.");
        }

        var entity = new CarritoEntity
        {
            UsuarioId = request.UsuarioId,
            Estado = "ACTIVO",
            FechaCreacion = DateTime.UtcNow
        };

        await _uow.Repository<CarritoEntity>().Add(entity);
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
