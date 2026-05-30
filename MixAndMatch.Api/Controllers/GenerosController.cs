using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Genero.Commands;
using MixAndMatch.Application.UseCases.Genero.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenerosController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return this.ToActionResult(await _mediator.Send(new GetAllGenerosQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetGeneroByIdQuery { GeneroId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GeneroRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new CreateGeneroCommand { NomGenero = dto.NomGenero }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] GeneroRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new UpdateGeneroCommand { GeneroId = id, NomGenero = dto.NomGenero }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteGeneroCommand { GeneroId = id }));
    }
}
