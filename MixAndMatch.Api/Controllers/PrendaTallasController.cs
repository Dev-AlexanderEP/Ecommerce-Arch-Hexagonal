using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.PrendaTalla.Commands;
using MixAndMatch.Application.UseCases.PrendaTalla.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrendaTallasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _mediator.Send(new GetAllPrendaTallasQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetPrendaTallaByIdQuery { PrendaTallaId = id }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PrendaTallaRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new CreatePrendaTallaCommand
        {
            PrendaId = dto.PrendaId,
            TallaId = dto.TallaId,
            Stock = dto.Stock
        }));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] PrendaTallaUpdateRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new UpdatePrendaTallaCommand
        {
            PrendaTallaId = id,
            Stock = dto.Stock
        }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeletePrendaTallaCommand { PrendaTallaId = id }));
}
