using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Carrito.Commands;
using MixAndMatch.Application.UseCases.Carrito.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarritosController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _mediator.Send(new GetAllCarritosQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetCarritoByIdQuery { CarritoId = id }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarritoRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new CreateCarritoCommand { UsuarioId = dto.UsuarioId }));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] CarritoRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new UpdateCarritoCommand { CarritoId = id, Estado = dto.Estado ?? string.Empty }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteCarritoCommand { CarritoId = id }));
}
