using MediatR;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Reporte.Queries;

public class GenerarReporteVentasQuery : IRequest<byte[]>
{
    public string Periodo { get; set; } = "diario";
}

public class GenerarReporteVentasQueryHandler(IReporteService reporteService)
    : IRequestHandler<GenerarReporteVentasQuery, byte[]>
{
    public Task<byte[]> Handle(GenerarReporteVentasQuery request, CancellationToken cancellationToken)
        => reporteService.GenerarReporteVentas(request.Periodo);
}
