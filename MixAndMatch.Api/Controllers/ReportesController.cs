using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Reporte.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = nameof(RolUsuario.ADMIN))]
public class ReportesController(IMediator _mediator) : ApiControllerBase
{
    private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    [HttpGet("stock")]
    public async Task<IActionResult> ReporteStock(
        [FromQuery] string? nombre,
        [FromQuery] string? genero,
        [FromQuery] string? categoria)
    {
        var bytes = await _mediator.Send(new GenerarReporteStockQuery
        {
            Nombre    = nombre,
            Genero    = genero,
            Categoria = categoria,
        });
        return File(bytes, ContentType, $"stock_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }

    [HttpGet("ventas")]
    public async Task<IActionResult> ReporteVentas([FromQuery] string periodo = "diario")
    {
        var bytes = await _mediator.Send(new GenerarReporteVentasQuery { Periodo = periodo });
        return File(bytes, ContentType, $"ventas_{periodo}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }
}
