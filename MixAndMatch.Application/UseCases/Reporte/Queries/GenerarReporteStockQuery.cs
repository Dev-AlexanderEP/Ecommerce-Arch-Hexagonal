using MediatR;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Reporte.Queries;

public class GenerarReporteStockQuery : IRequest<byte[]>
{
    public string? Nombre    { get; set; }
    public string? Genero    { get; set; }
    public string? Categoria { get; set; }
}

public class GenerarReporteStockQueryHandler(IReporteService reporteService)
    : IRequestHandler<GenerarReporteStockQuery, byte[]>
{
    public Task<byte[]> Handle(GenerarReporteStockQuery request, CancellationToken cancellationToken)
        => reporteService.GenerarReporteStock(request.Nombre, request.Genero, request.Categoria);
}
