using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.DescuentoUsuario.Commands;
using MixAndMatch.Application.UseCases.DescuentoUsuario.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DescuentoUsuariosController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllDescuentoUsuariosQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetDescuentoUsuarioByIdQuery { DescuentoUsuarioId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDescuentoUsuarioCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteDescuentoUsuarioCommand { DescuentoUsuarioId = id }));
    }
}
