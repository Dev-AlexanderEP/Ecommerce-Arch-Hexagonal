using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.PrendaTalla.Commands;
using MixAndMatch.Application.UseCases.PrendaTalla.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrendaTallasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _mediator.Send(new GetAllPrendaTallasQuery { Page = page, PageSize = pageSize }));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetPrendaTallaByIdQuery { PrendaTallaId = id }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePrendaTallaCommand command) =>
        this.ToActionResult(await _mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePrendaTallaCommand command)
    {
        command.PrendaTallaId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeletePrendaTallaCommand { PrendaTallaId = id }));
}
