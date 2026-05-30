using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Application.UseCases.Prenda.Commands;
using MixAndMatch.Application.UseCases.Prenda.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrendasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllPrendasQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return Ok(await _mediator.Send(new GetPrendaByIdQuery { PrendaId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PrendaRequestDto dto)
    {
        return Ok(await _mediator.Send(new CreatePrendaCommand
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            MarcaId = dto.MarcaId,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            GeneroId = dto.GeneroId,
            Precio = dto.Precio,
            Activo = dto.Activo
        }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] PrendaRequestDto dto)
    {
        return Ok(await _mediator.Send(new UpdatePrendaCommand
        {
            PrendaId = id,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            MarcaId = dto.MarcaId,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            GeneroId = dto.GeneroId,
            Precio = dto.Precio,
            Activo = dto.Activo
        }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return Ok(await _mediator.Send(new DeletePrendaCommand { PrendaId = id }));
    }
}
