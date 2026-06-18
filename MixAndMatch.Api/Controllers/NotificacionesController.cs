using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Notificacion.Commands;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController(IMediator _mediator) : ControllerBase
{
    [HttpPost("bienvenida")]
    public async Task<IActionResult> SendBienvenida([FromBody] SendWelcomeEmailCommand command)
        => this.ToActionResult(await _mediator.Send(command));

    [HttpPost("recuperar-contrasenia")]
    public async Task<IActionResult> RecuperarContrasenia([FromBody] SendRecuperacionContraseniaCommand command)
        => this.ToActionResult(await _mediator.Send(command));

    [HttpPost("verificar-otp")]
    public async Task<IActionResult> VerificarOtp([FromBody] VerificarOtpRecuperacionCommand command)
        => this.ToActionResult(await _mediator.Send(command));
}
