using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.CarritoItem.Commands;
using MixAndMatch.Application.UseCases.CarritoItem.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarritoItemsController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _mediator.Send(new GetAllCarritoItemsQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) =>
        this.ToActionResult(await _mediator.Send(new GetCarritoItemByIdQuery { CarritoItemId = id }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarritoItemRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new CreateCarritoItemCommand
        {
            CarritoId = dto.CarritoId,
            PrendaTallaId = dto.PrendaTallaId,
            PrecioUnitario = dto.PrecioUnitario,
            Cantidad = dto.Cantidad
        }));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] CarritoItemRequestDto dto) =>
        this.ToActionResult(await _mediator.Send(new UpdateCarritoItemCommand
        {
            CarritoItemId = id,
            PrecioUnitario = dto.PrecioUnitario,
            Cantidad = dto.Cantidad
        }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id) =>
        this.ToActionResult(await _mediator.Send(new DeleteCarritoItemCommand { CarritoItemId = id }));
}
