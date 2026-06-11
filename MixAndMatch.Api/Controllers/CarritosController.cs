using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Carrito.Commands;
using MixAndMatch.Application.UseCases.Carrito.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarritosController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _mediator.Send(new GetAllCarritosQuery { Page = page, PageSize = pageSize }));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetCarritoByIdQuery { CarritoId = id }));

[HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCarritoCommand command) =>
        this.ToActionResult(await _mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCarritoCommand command)
    {
        command.CarritoId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteCarritoCommand { CarritoId = id }));
}
