using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetEnvioByTrackingNumberQuery : IRequest<ApiResponse<EnvioResponseDto>>
{
    public required string TrackingNumber { get; set; }
}

public class GetEnvioByTrackingNumberQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetEnvioByTrackingNumberQuery, ApiResponse<EnvioResponseDto>>
{
    public async Task<ApiResponse<EnvioResponseDto>> Handle(GetEnvioByTrackingNumberQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Envios.FindByTrackingNumber(request.TrackingNumber);
        if (entity is null)
            return ApiResponse<EnvioResponseDto>.Fail($"Envío no encontrado para tracking number '{request.TrackingNumber}'.", ErrorType.NotFound);

        return ApiResponse<EnvioResponseDto>.Ok(new EnvioResponseDto
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
