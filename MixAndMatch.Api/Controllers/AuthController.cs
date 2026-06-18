using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Auth.Commands;
using MixAndMatch.Application.UseCases.Auth.Queries;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
public class AuthController(IMediator _mediator) : ApiControllerBase
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

    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginGoogle([FromBody] string idToken)
    {
        return this.ToActionResult(await _mediator.Send(new LoginGoogleQuery { IdToken = idToken }));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        command.UsuarioId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }
}
