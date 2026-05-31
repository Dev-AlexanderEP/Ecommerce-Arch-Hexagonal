using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.PrendaImagen.Commands;
using MixAndMatch.Application.UseCases.PrendaImagen.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrendaImagenesController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _mediator.Send(new GetAllPrendaImagenesQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetPrendaImagenByIdQuery { PrendaImagenId = id }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PrendaImagenRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new CreatePrendaImagenCommand
        {
            PrendaId = dto.PrendaId,
            Tipo = dto.Tipo,
            Url = dto.Url,
            Orden = dto.Orden
        }));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] PrendaImagenRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new UpdatePrendaImagenCommand
        {
            PrendaImagenId = id,
            Tipo = dto.Tipo,
            Url = dto.Url,
            Orden = dto.Orden
        }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeletePrendaImagenCommand { PrendaImagenId = id }));
}
