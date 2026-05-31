using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Application.UseCases.DatosEnvio.Commands;
using MixAndMatch.Application.UseCases.DatosEnvio.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatosEnvioController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllDatosEnvioQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetDatosEnvioByIdQuery
        {
            DatosEnvioId = id
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDatosEnvioCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateDatosEnvioCommand command)
    {
        command.DatosEnvioId = id;

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _mediator.Send(new DeleteDatosEnvioCommand
        {
            DatosEnvioId = id
        });

        return Ok(result);
    }
}