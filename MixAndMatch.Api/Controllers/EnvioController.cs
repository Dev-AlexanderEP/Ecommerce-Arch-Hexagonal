using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Envio.Commands;
using MixAndMatch.Application.UseCases.Envio.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnvioController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllEnviosQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetEnvioByIdQuery { EnvioId = id }));
    }

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Create([FromBody] CreateEnvioCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateEnvioCommand command)
    {
        command.EnvioId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteEnvioCommand { EnvioId = id }));
    }

    [HttpGet("tracking/{trackingNumber}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetByTrackingNumber(string trackingNumber) =>
        this.ToActionResult(await _mediator.Send(new GetEnvioByTrackingNumberQuery { TrackingNumber = trackingNumber }));

    [HttpGet("usuario/{userId}/entregados")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetEntregadosPorUsuario(long userId) =>
        this.ToActionResult(await _mediator.Send(new GetEnviosEntregadosPorUsuarioQuery { UsuarioId = userId }));

    [HttpGet("usuario/{userId}/no-entregados")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetNoEntregadosPorUsuario(long userId) =>
        this.ToActionResult(await _mediator.Send(new GetEnviosNoEntregadosPorUsuarioQuery { UsuarioId = userId }));
}
