using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.VentasDetalle.Commands;
using MixAndMatch.Application.UseCases.VentasDetalle.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class VentasDetallesController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllVentasDetallesQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetVentasDetalleByIdQuery
        {
            VentasDetalleId = id,
            SolicitanteId = CurrentUser.Id,
            EsAdmin = CurrentUser.IsAdmin
        }));
    }

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Create([FromBody] CreateVentasDetalleCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateVentasDetalleCommand command)
    {
        command.VentasDetalleId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteVentasDetalleCommand { VentasDetalleId = id }));
    }
}
