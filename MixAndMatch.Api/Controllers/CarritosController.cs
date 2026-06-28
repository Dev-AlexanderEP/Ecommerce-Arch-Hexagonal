using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Carrito.Commands;
using MixAndMatch.Application.UseCases.Carrito.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class CarritosController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _mediator.Send(new GetAllCarritosQuery { Page = page, PageSize = pageSize }));

    [HttpGet("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetCarritoByIdQuery
        {
            CarritoId = id,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Create() =>
        this.ToActionResult(await _mediator.Send(new CreateCarritoCommand { UsuarioId = CurrentUser.Id }));

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Update(long id, [FromBody] string estado) =>
        this.ToActionResult(await _mediator.Send(new UpdateCarritoCommand
        {
            CarritoId = id,
            Estado = estado,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteCarritoCommand
        {
            CarritoId = id,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpGet("abierto/usuario/{usuarioId}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetCarritosAbiertosByUsuario(long usuarioId) =>
        this.ToActionResult(await _mediator.Send(new GetCarritosAbiertosByUsuarioQuery
        {
            UsuarioId = usuarioId
        }));

    [HttpGet("{id}/cantidad-items")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetCantidadItems(long id) =>
        this.ToActionResult(await _mediator.Send(new GetCantidadItemsCarritoQuery
        {
            CarritoId = id
        }));
}
