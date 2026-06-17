using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Commands;

public class CreateVentaCommand : IRequest<ApiResponse<VentaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }
}

public class CreateVentaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateVentaCommand, ApiResponse<VentaResponseDto>>
{
    public async Task<ApiResponse<VentaResponseDto>> Handle(CreateVentaCommand request, CancellationToken cancellationToken)
    {
        var entity = new VentaEntity
        {
            UsuarioId = request.SolicitanteId,
            Estado = EstadoVenta.PENDIENTE,
            FechaCreacion = DateTime.UtcNow
        };

        await _uow.Ventas.Add(entity);
        await _uow.Complete();

        return ApiResponse<VentaResponseDto>.Created(new VentaResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado?.ToString(),
            UpdatedAt = entity.UpdatedAt
        });
    }
}
