using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Venta.Commands;
using MixAndMatch.Application.UseCases.Venta.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/ventas")]
public class VentasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return this.ToActionResult(await _mediator.Send(new GetAllVentasQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetVentaByIdQuery { VentaId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VentaRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new CreateVentaCommand { UsuarioId = dto.UsuarioId }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] VentaRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new UpdateVentaCommand { VentaId = id, Estado = dto.Estado ?? string.Empty }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteVentaCommand { VentaId = id }));
    }
}
