using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Application.UseCases.Categoria.Commands;
using MixAndMatch.Application.UseCases.Categoria.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllCategoriasQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return Ok(await _mediator.Send(new GetCategoriaByIdQuery { CategoriaId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoriaRequestDto dto)
    {
        return Ok(await _mediator.Send(new CreateCategoriaCommand { NomCategoria = dto.NomCategoria }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] CategoriaRequestDto dto)
    {
        return Ok(await _mediator.Send(new UpdateCategoriaCommand { CategoriaId = id, NomCategoria = dto.NomCategoria }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return Ok(await _mediator.Send(new DeleteCategoriaCommand { CategoriaId = id }));
    }
}
