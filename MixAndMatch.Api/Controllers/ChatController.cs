using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Chat.Queries;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
public class ChatController(IMediator _mediator) : ApiControllerBase
{
    [HttpPost("preguntar")]
    public async Task<IActionResult> Preguntar([FromBody] PreguntarChatQuery query) =>
        this.ToActionResult(await _mediator.Send(query));
}
