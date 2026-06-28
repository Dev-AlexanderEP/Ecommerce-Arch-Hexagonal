using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.CarritoItem.Commands;
using MixAndMatch.Application.UseCases.CarritoItem.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class CarritoItemsController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _mediator.Send(new GetAllCarritoItemsQuery { Page = page, PageSize = pageSize }));

    [HttpGet("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetCarritoItemByIdQuery
        {
            CarritoItemId = id,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Create([FromBody] CreateCarritoItemCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCarritoItemCommand command)
    {
        command.CarritoItemId = id;
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteCarritoItemCommand
        {
            CarritoItemId = id,
            SolicitanteId = CurrentUser.Id
        }));

    [HttpPost("agregar")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> AgregarOIncrementar([FromQuery] long carritoId, [FromQuery] long prendaId, [FromQuery] long tallaId)
    {
        var command = new AgregarOIncrementarCarritoItemCommand
        {
            CarritoId = carritoId,
            PrendaId = prendaId,
            TallaId = tallaId,
            SolicitanteId = CurrentUser.Id
        };
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}/cantidad")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> ActualizarCantidad(long id, [FromQuery] int cantidad)
    {
        var command = new ActualizarCantidadCarritoItemCommand
        {
            CarritoItemId = id,
            Cantidad = cantidad,
            SolicitanteId = CurrentUser.Id
        };
        return this.ToActionResult(await _mediator.Send(command));
    }
}
