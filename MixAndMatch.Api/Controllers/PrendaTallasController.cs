using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.PrendaTalla.Commands;
using MixAndMatch.Application.UseCases.PrendaTalla.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class PrendaTallasController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _mediator.Send(new GetAllPrendaTallasQuery { Page = page, PageSize = pageSize }));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetPrendaTallaByIdQuery { PrendaTallaId = id }));

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Create([FromBody] CreatePrendaTallaCommand command) =>
        this.ToActionResult(await _mediator.Send(command));

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePrendaTallaCommand command)
    {
        command.PrendaTallaId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeletePrendaTallaCommand { PrendaTallaId = id }));

    [HttpPut("stock/decremento")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> RestarUnoStock([FromQuery] RestarUnoStockCommand command) =>
        this.ToActionResult(await _mediator.Send(command));

    [HttpPut("stock/incremento")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> SumarUnoStock([FromQuery] SumarUnoStockCommand command) =>
        this.ToActionResult(await _mediator.Send(command));

    [HttpPut("stock/suma")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> SumarStock([FromQuery] SumarStockCommand command) =>
        this.ToActionResult(await _mediator.Send(command));
}
