using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class CreateVentaCommand : IRequest<ApiResponse<VentaResponseDto>>
{
    public required long UsuarioId { get; set; }
}

public class CreateVentaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateVentaCommand, ApiResponse<VentaResponseDto>>
{
    public async Task<ApiResponse<VentaResponseDto>> Handle(CreateVentaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Repository<UsuarioEntity>().GetById(request.UsuarioId);
        if (usuario is null)
        {
            return ApiResponse<VentaResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        var entity = new VentaEntity
        {
            UsuarioId = request.UsuarioId,
            Estado = "PENDIENTE",
            FechaCreacion = DateTime.UtcNow
        };

        await _uow.Repository<VentaEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponse<VentaResponseDto>.Ok(new VentaResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
