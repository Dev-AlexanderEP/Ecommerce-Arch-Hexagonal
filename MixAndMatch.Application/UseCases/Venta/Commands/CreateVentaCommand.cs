using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class CreateVentaCommand : IRequest<ApiResponseDto<VentaResponseDto>>
{
    public required long UsuarioId { get; set; }
}

public class CreateVentaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateVentaCommand, ApiResponseDto<VentaResponseDto>>
{
    public async Task<ApiResponseDto<VentaResponseDto>> Handle(CreateVentaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Repository<UsuarioEntity>().GetById(request.UsuarioId);
        if (usuario is null)
        {
            return ApiResponseDto<VentaResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        var entity = new VentaEntity
        {
            UsuarioId = request.UsuarioId,
            Estado = "PENDIENTE",
            FechaCreacion = DateTime.UtcNow
        };

        await _uow.Repository<VentaEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponseDto<VentaResponseDto>.Ok(new VentaResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
