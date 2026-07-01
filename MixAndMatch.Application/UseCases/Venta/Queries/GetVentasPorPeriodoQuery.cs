using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetVentasPorPeriodoQuery : IRequest<ApiResponse<List<VentasPorPeriodoDto>>>
{
    /// <summary>diario | semanal | mensual | anual</summary>
    public string Agrupacion { get; set; } = "diario";
}

public class GetVentasPorPeriodoQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetVentasPorPeriodoQuery, ApiResponse<List<VentasPorPeriodoDto>>>
{
    public async Task<ApiResponse<List<VentasPorPeriodoDto>>> Handle(GetVentasPorPeriodoQuery request, CancellationToken ct)
        => ApiResponse<List<VentasPorPeriodoDto>>.Ok(await _uow.Ventas.GetVentasPorPeriodo(request.Agrupacion));
}
