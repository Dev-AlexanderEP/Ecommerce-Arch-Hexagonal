using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Usuario.Commands;
using MixAndMatch.Application.UseCases.Usuario.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return this.ToActionResult(await _mediator.Send(new GetAllUsuariosQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetUsuarioByIdQuery { UsuarioId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUsuarioCommand command)
    {
        command.UsuarioId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteUsuarioCommand { UsuarioId = id }));
    }
}
