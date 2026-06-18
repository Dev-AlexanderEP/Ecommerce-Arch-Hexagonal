using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Notificacion.Commands;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificacionesController(IMediator _mediator) : ControllerBase
{
    [HttpPost("bienvenida")]
    [AllowAnonymous]
    public async Task<IActionResult> SendBienvenida([FromBody] SendWelcomeEmailCommand command)
        => this.ToActionResult(await _mediator.Send(command));

    [HttpPost("recuperar-contrasenia")]
    [AllowAnonymous]
    public async Task<IActionResult> RecuperarContrasenia([FromBody] SendRecuperacionContraseniaCommand command)
        => this.ToActionResult(await _mediator.Send(command));

    [HttpPost("verificar-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerificarOtp([FromBody] VerificarOtpRecuperacionCommand command)
        => this.ToActionResult(await _mediator.Send(command));

    [HttpPost("confirmacion-venta")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> ConfirmacionVenta([FromBody] SendConfirmacionVentaEmailCommand command)
        => this.ToActionResult(await _mediator.Send(command));
}
