using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Commands;

public class CreateEnvioCommand : IRequest<ApiResponse<EnvioResponseDto>>
{
    public required long VentaId { get; set; }
    public required long DatosEnvioId { get; set; }
    public required decimal CostoEnvio { get; set; }
    public required DateOnly FechaEnvio { get; set; }
    public DateOnly? FechaEntrega { get; set; }
    public required string Estado { get; set; }
    public required string MetodoEnvio { get; set; }
    public string? TrackingNumber { get; set; }
}

public class CreateEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateEnvioCommand, ApiResponse<EnvioResponseDto>>
{
    public async Task<ApiResponse<EnvioResponseDto>> Handle(CreateEnvioCommand request, CancellationToken cancellationToken)
    {
        var venta = await _uow.Ventas.GetById(request.VentaId);
        if (venta is null)
        {
            return ApiResponse<EnvioResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.Validation);
        }

        var datosEnvio = await _uow.DatosEnvios.GetById(request.DatosEnvioId);
        if (datosEnvio is null)
        {
            return ApiResponse<EnvioResponseDto>.Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.", ErrorType.Validation);
        }

        if (await _uow.Envios.ExisteParaVenta(request.VentaId))
        {
            return ApiResponse<EnvioResponseDto>.Fail($"La venta {request.VentaId} ya tiene un envío registrado.", ErrorType.Conflict);
        }

        // El formato del estado ya lo valida CreateEnvioCommandValidator (400); esto es defensa.
        if (!Enum.TryParse<EstadoEnvio>(request.Estado, ignoreCase: true, out var estadoEnvio))
        {
            return ApiResponse<EnvioResponseDto>.Fail($"Estado inválido: {request.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoEnvio>())}.", ErrorType.Validation);
        }

        var entity = new EnvioEntity
        {
            VentaId = request.VentaId,
            DatosEnvioId = request.DatosEnvioId,
            CostoEnvio = request.CostoEnvio,
            FechaEnvio = request.FechaEnvio,
            FechaEntrega = request.FechaEntrega,
            Estado = estadoEnvio,
            MetodoEnvio = request.MetodoEnvio,
            TrackingNumber = request.TrackingNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Envios.Add(entity);
        await _uow.Complete();

        return ApiResponse<EnvioResponseDto>.Created(new EnvioResponseDto
        {
            Id = entity.Id,
            VentaId = entity.VentaId,
            DatosEnvioId = entity.DatosEnvioId,
            CostoEnvio = entity.CostoEnvio,
            FechaEnvio = entity.FechaEnvio,
            FechaEntrega = entity.FechaEntrega,
            Estado = entity.Estado.ToString(),
            MetodoEnvio = entity.MetodoEnvio,
            TrackingNumber = entity.TrackingNumber,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
