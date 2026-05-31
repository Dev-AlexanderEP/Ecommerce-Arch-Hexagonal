
using MediatR;
using Microsoft.AspNetCore.Mvc;

using MixAndMatch.Application.UseCases.Pago.Commands;
using MixAndMatch.Application.UseCases.Pago.Queries;

namespace MixAndMatch.Api.Controllers;




[ApiController]
[Route("api/[controller]")]
public class PagoController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllPagosQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetPagoByIdQuery
        {
            Id = id
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePagoCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdatePagoCommand command)
    {
        command.Id = id;

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _mediator.Send(new DeletePagoCommand
        {
            Id = id
        });

        return Ok(result);
    }
}