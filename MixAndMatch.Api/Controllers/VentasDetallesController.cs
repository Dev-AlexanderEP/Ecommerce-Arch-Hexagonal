using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.VentasDetalle.Commands;
using MixAndMatch.Application.UseCases.VentasDetalle.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/ventasdetalles")]
public class VentasDetallesController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return this.ToActionResult(await _mediator.Send(new GetAllVentasDetallesQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetVentasDetalleByIdQuery { VentasDetalleId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VentasDetalleRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new CreateVentasDetalleCommand
        {
            VentaId = dto.VentaId,
            PrendaTallaId = dto.PrendaTallaId,
            Cantidad = dto.Cantidad,
            PrecioUnitario = dto.PrecioUnitario
        }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] VentasDetalleRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new UpdateVentasDetalleCommand
        {
            VentasDetalleId = id,
            Cantidad = dto.Cantidad,
            PrecioUnitario = dto.PrecioUnitario
        }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteVentasDetalleCommand { VentasDetalleId = id }));
    }
}
