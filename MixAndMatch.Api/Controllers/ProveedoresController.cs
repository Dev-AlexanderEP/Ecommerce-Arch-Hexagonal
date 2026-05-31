using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Proveedor.Commands;
using MixAndMatch.Application.UseCases.Proveedor.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProveedoresController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return this.ToActionResult(await _mediator.Send(new GetAllProveedoresQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetProveedorByIdQuery { ProveedorId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProveedorCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateProveedorCommand command)
    {
        command.ProveedorId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteProveedorCommand { ProveedorId = id }));
    }
}
