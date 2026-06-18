using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Resenias.Commands;
using MixAndMatch.Application.UseCases.Resenias.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ReseniasController(IMediator _mediator) : ApiControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Create([FromBody] CreateReseniaCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? prendaId,
        [FromQuery] long? usuarioId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetReseniasQuery
        {
            PrendaId = prendaId,
            UsuarioId = usuarioId,
            Page = page,
            PageSize = pageSize
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetReseniaByIdQuery { ReseniaId = id }));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateReseniaCommand command)
    {
        command.ReseniaId = id;
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteReseniaCommand
        {
            ReseniaId = id,
            SolicitanteId = CurrentUser.Id,
            EsAdmin = CurrentUser.IsAdmin
        }));
    }

    [HttpPatch("{id}/estado")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> UpdateEstado(long id, [FromBody] UpdateEstadoReseniaCommand command)
    {
        command.ReseniaId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }
}
