using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Venta.Commands;
using MixAndMatch.Application.UseCases.Venta.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class VentasController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? nombreUsuario,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllVentasQuery
        {
            NombreUsuario = nombreUsuario,
            Page = page,
            PageSize = pageSize
        }));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetVentaByIdQuery
        {
            VentaId = id,
            SolicitanteId = CurrentUser.Id,
            EsAdmin = CurrentUser.IsAdmin
        }));
    }

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Create([FromBody] CreateVentaCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateVentaCommand command)
    {
        command.VentaId = id;
        command.SolicitanteId = CurrentUser.Id;
        command.EsAdmin = CurrentUser.IsAdmin;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteVentaCommand
        {
            VentaId = id,
            SolicitanteId = CurrentUser.Id,
            EsAdmin = CurrentUser.IsAdmin
        }));
    }

    [HttpGet("segunda-pendiente")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> GetSegundaPendiente()
    {
        return this.ToActionResult(await _mediator.Send(new GetSegundaVentaPendienteQuery
        {
            UsuarioId = CurrentUser.Id
        }));
    }

    [HttpPost("carrito-detalle")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> AgregarDetallesDesdeCarrito([FromBody] AgregarDetallesDesdeCarritoCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpGet("total")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetTotal() =>
        this.ToActionResult(await _mediator.Send(new GetTotalVentasQuery()));

    [HttpGet("por-periodo")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetPorPeriodo([FromQuery] string agrupacion = "diario") =>
        this.ToActionResult(await _mediator.Send(new GetVentasPorPeriodoQuery { Agrupacion = agrupacion }));

    [HttpGet("por-genero")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetPorGenero() =>
        this.ToActionResult(await _mediator.Send(new GetVentasPorGeneroQuery()));
}
