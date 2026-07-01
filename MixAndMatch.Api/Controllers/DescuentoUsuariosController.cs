using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.DescuentoUsuario.Commands;
using MixAndMatch.Application.UseCases.DescuentoUsuario.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class DescuentoUsuariosController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{nameof(RolUsuario.CLIENTE)},{nameof(RolUsuario.ADMIN)}")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _mediator.Send(new GetAllDescuentoUsuariosQuery
        {
            Page = page,
            PageSize = pageSize,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpGet("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.CLIENTE)},{nameof(RolUsuario.ADMIN)}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetDescuentoUsuarioByIdQuery
        {
            DescuentoUsuarioId = id,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Create([FromBody] CreateDescuentoUsuarioCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteDescuentoUsuarioCommand
        {
            DescuentoUsuarioId = id,
            SolicitanteId = CurrentUser.Id
        }));
}
