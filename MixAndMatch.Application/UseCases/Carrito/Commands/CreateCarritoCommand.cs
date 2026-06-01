using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Carrito.Commands;

public class CreateCarritoCommand : IRequest<ApiResponseDto<CarritoResponseDto>>
{
    public required long UsuarioId { get; set; }
}

public class CreateCarritoCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateCarritoCommand, ApiResponseDto<CarritoResponseDto>>
{
    public async Task<ApiResponseDto<CarritoResponseDto>> Handle(CreateCarritoCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Repository<UsuarioEntity>().GetById(request.UsuarioId);
        if (usuario is null)
        {
            return ApiResponseDto<CarritoResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        var carritos = await _uow.Repository<CarritoEntity>().GetAll();
        if (carritos.Any(x => x.UsuarioId == request.UsuarioId && x.Estado == "ACTIVO"))
        {
            return ApiResponseDto<CarritoResponseDto>.Fail("El usuario ya tiene un carrito activo.");
        }

        var entity = new CarritoEntity
        {
            UsuarioId = request.UsuarioId,
            Estado = "ACTIVO",
            FechaCreacion = DateTime.UtcNow
        };

        await _uow.Repository<CarritoEntity>().Add(entity);
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
