using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;
using MixAndMatch.Application.UseCases.DescuentoCategoria.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DescuentoCategoriasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllDescuentoCategoriasQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetDescuentoCategoriaByIdQuery { DescuentoCategoriaId = id }));
    }

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Create([FromBody] CreateDescuentoCategoriaCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDescuentoCategoriaCommand command)
    {
        command.DescuentoCategoriaId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteDescuentoCategoriaCommand { DescuentoCategoriaId = id }));
    }
}
