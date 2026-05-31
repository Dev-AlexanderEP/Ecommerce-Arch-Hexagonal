using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Talla.Commands;
using MixAndMatch.Application.UseCases.Talla.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TallasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _mediator.Send(new GetAllTallasQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetTallaByIdQuery { TallaId = id }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TallaRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new CreateTallaCommand { NomTalla = dto.NomTalla }));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] TallaRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new UpdateTallaCommand { TallaId = id, NomTalla = dto.NomTalla }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteTallaCommand { TallaId = id }));
}
