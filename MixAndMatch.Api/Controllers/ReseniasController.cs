using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Resenias.Commands;
using MixAndMatch.Application.UseCases.Resenias.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReseniasController(IMediator _mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReseniaCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? prendaId,
        [FromQuery] long? usuarioId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (prendaId.HasValue == usuarioId.HasValue)
        {
            return BadRequest(ApiResponseDto<string>.Fail("Debe enviar prendaId o usuarioId."));
        }

        if (prendaId.HasValue)
        {
            return this.ToActionResult(await _mediator.Send(new GetReseniasByPrendaQuery
            {
                PrendaId = prendaId.Value,
                Page = page,
                PageSize = pageSize
            }));
        }

        return this.ToActionResult(await _mediator.Send(new GetReseniasByUsuarioQuery
        {
            UsuarioId = usuarioId!.Value
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetReseniaByIdQuery { ReseniaId = id }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateReseniaCommand command)
    {
        command.ReseniaId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteReseniaCommand { ReseniaId = id }));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> UpdateEstado(long id, [FromBody] UpdateEstadoReseniaCommand command)
    {
        command.ReseniaId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }
}
