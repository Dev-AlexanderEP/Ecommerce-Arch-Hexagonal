using MediatR;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Proveedor.Commands;
using MixAndMatch.Application.UseCases.Proveedor.Queries;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProveedoresController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return this.ToActionResult(await _mediator.Send(new GetAllProveedoresQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetProveedorByIdQuery { ProveedorId = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProveedorRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new CreateProveedorCommand { NomProveedor = dto.NomProveedor }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] ProveedorRequestDto dto)
    {
        return this.ToActionResult(await _mediator.Send(new UpdateProveedorCommand { ProveedorId = id, NomProveedor = dto.NomProveedor }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeleteProveedorCommand { ProveedorId = id }));
    }
}
