using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;
using MixAndMatch.Application.UseCases.DescuentoCategoria.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DescuentoCategoriasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllDescuentoCategoriasQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return Ok(await _mediator.Send(new GetDescuentoCategoriaByIdQuery { DescuentoCategoriaId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DescuentoCategoriaRequestDto dto)
    {
        return Ok(await _mediator.Send(new CreateDescuentoCategoriaCommand
        {
            CategoriaId = dto.CategoriaId,
            Porcentaje = dto.Porcentaje,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Activo = dto.Activo
        }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] DescuentoCategoriaRequestDto dto)
    {
        return Ok(await _mediator.Send(new UpdateDescuentoCategoriaCommand
        {
            DescuentoCategoriaId = id,
            CategoriaId = dto.CategoriaId,
            Porcentaje = dto.Porcentaje,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Activo = dto.Activo
        }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return Ok(await _mediator.Send(new DeleteDescuentoCategoriaCommand { DescuentoCategoriaId = id }));
    }
}
