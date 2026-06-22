using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetEnviosEntregadosPorUsuarioQuery : IRequest<ApiResponse<IEnumerable<EnvioResponseDto>>>
{
    public required long UsuarioId { get; set; }
}

public class GetEnviosEntregadosPorUsuarioQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetEnviosEntregadosPorUsuarioQuery, ApiResponse<IEnumerable<EnvioResponseDto>>>
{
    public async Task<ApiResponse<IEnumerable<EnvioResponseDto>>> Handle(GetEnviosEntregadosPorUsuarioQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Envios.GetByUsuarioYEstado(request.UsuarioId, EstadoEnvio.ENTREGADO);

        return ApiResponse<IEnumerable<EnvioResponseDto>>.Ok(items.Select(e => new EnvioResponseDto
        {
            Id = e.Id,
            VentaId = e.VentaId,
            DatosEnvioId = e.DatosEnvioId,
            CostoEnvio = e.CostoEnvio,
            FechaEnvio = e.FechaEnvio,
            FechaEntrega = e.FechaEntrega,
            Estado = e.Estado.ToString(),
            MetodoEnvio = e.MetodoEnvio,
            TrackingNumber = e.TrackingNumber,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }));
    }
}
