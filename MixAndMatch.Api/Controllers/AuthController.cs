using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Auth.Commands;
using MixAndMatch.Application.UseCases.Auth.Queries;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator _mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUsuarioCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUsuarioQuery query)
    {
        return this.ToActionResult(await _mediator.Send(query));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        // El id viene del token, nunca del body
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(sub, out var usuarioId))
            return Unauthorized();

        command.UsuarioId = usuarioId;
        return this.ToActionResult(await _mediator.Send(command));
    }
}
