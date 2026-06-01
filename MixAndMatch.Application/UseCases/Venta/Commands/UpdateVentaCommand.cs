using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class UpdateVentaCommand : IRequest<ApiResponseDto<VentaResponseDto>>
{
    public required long VentaId { get; set; }
    public required string Estado { get; set; }
}

public class UpdateVentaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateVentaCommand, ApiResponseDto<VentaResponseDto>>
{
    private static readonly HashSet<string> EstadosValidos = ["PENDIENTE", "PROCESANDO", "ENVIADO", "ENTREGADO", "CANCELADO"];

    public async Task<ApiResponseDto<VentaResponseDto>> Handle(UpdateVentaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<VentaEntity>();
        var entity = await repo.GetById(request.VentaId);
        if (entity is null)
        {
            return ApiResponseDto<VentaResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        var estado = (request.Estado ?? string.Empty).Trim().ToUpperInvariant();
        if (!EstadosValidos.Contains(estado))
        {
            return ApiResponseDto<VentaResponseDto>.Fail($"Estado inválido. Valores permitidos: {string.Join(", ", EstadosValidos)}.");
        }

        entity.Estado = estado;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
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
