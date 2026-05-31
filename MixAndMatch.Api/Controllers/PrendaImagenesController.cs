using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.PrendaImagen.Commands;
using MixAndMatch.Application.UseCases.PrendaImagen.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrendaImagenesController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _mediator.Send(new GetAllPrendaImagenesQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetPrendaImagenByIdQuery { PrendaImagenId = id }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePrendaImagenCommand command) =>
        this.ToActionResult(await _mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePrendaImagenCommand command)
    {
        command.PrendaImagenId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeletePrendaImagenCommand { PrendaImagenId = id }));
}
