using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.VentasDetalle.Commands;
using MixAndMatch.Application.UseCases.VentasDetalle.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/ventasdetalles")]
public class VentasDetallesController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllVentasDetallesQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetVentasDetalleByIdQuery { VentasDetalleId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVentasDetalleCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateVentasDetalleCommand command)
    {
        command.VentasDetalleId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteVentasDetalleCommand { VentasDetalleId = id }));
    }
}
