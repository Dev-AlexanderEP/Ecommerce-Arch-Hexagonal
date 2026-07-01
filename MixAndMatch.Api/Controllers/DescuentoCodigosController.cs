using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;
using MixAndMatch.Application.UseCases.DescuentoCodigo.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class DescuentoCodigosController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllDescuentoCodigosQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetDescuentoCodigoByIdQuery { DescuentoCodigoId = id }));
    }

    [HttpGet("codigo/{codigo}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> GetByCodigo(string codigo)
    {
        return this.ToActionResult(await _mediator.Send(new GetDescuentoCodigoByCodigoQuery { Codigo = codigo }));
    }

    [HttpDelete("codigo/{codigo}/quitar")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Quitar(string codigo) =>
        this.ToActionResult(await _mediator.Send(new QuitarDescuentoCodigoCommand
        {
            Codigo = codigo,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpPost("codigo/{codigo}/aplicar")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Aplicar(string codigo) =>
        this.ToActionResult(await _mediator.Send(new AplicarDescuentoCodigoCommand
        {
            Codigo = codigo,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Create([FromBody] CreateDescuentoCodigoCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDescuentoCodigoCommand command)
    {
        command.DescuentoCodigoId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteDescuentoCodigoCommand { DescuentoCodigoId = id }));
    }
}
