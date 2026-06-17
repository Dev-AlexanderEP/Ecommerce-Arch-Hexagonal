using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetEnvioByIdQuery : IRequest<ApiResponse<EnvioResponseDto>>
{
    public required long EnvioId { get; set; }
}

public class GetEnvioByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetEnvioByIdQuery, ApiResponse<EnvioResponseDto>>
{
    public async Task<ApiResponse<EnvioResponseDto>> Handle(GetEnvioByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Envios.GetById(request.EnvioId);
        if (entity is null)
        {
            return ApiResponse<EnvioResponseDto>.Fail($"Envío no encontrado para id {request.EnvioId}.");
        }

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
