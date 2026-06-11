using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Application.UseCases.MetodoPago.Commands;
using MixAndMatch.Application.UseCases.MetodoPago.Queries;
using MixAndMatch.Application.UseCases.MetodoPagoUseCase.Commands;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetodoPagoController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllMetodoPagoQuery { Page = page, PageSize = pageSize });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetMetodoPagoByIdQuery
        {
            Id = id
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMetodoPagoCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateMetodoPagoCommand command)
    {
        command.Id = id;

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _mediator.Send(new DeleteMetodoPagoCommand
        {
            Id = id
        });

        return Ok(result);
    }
}