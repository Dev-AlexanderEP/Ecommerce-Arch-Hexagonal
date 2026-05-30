using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Marca.Commands;
using MixAndMatch.Application.UseCases.Marca.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return this.ToActionResult(await _mediator.Send(new GetAllMarcasQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetMarcaByIdQuery { MarcaId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MarcaRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new CreateMarcaCommand { NomMarca = dto.NomMarca }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] MarcaRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new UpdateMarcaCommand { MarcaId = id, NomMarca = dto.NomMarca }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteMarcaCommand { MarcaId = id }));
    }
}
