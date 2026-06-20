using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Prenda.Commands;
using MixAndMatch.Application.UseCases.Prenda.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrendasController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllPrendasQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetPrendaByIdQuery { PrendaId = id }));
    }

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Create([FromBody] CreatePrendaCommand command)
    {
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePrendaCommand command)
    {
        command.PrendaId = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeletePrendaCommand { PrendaId = id }));
    }

    [HttpGet("tallas-por-categoria/{categoria}")]
    public async Task<IActionResult> GetTallasPorCategoria(string categoria) =>
        this.ToActionResult(await _mediator.Send(new GetTallasPorCategoriaQuery { Categoria = categoria }));

    [HttpGet("marcas-por-categoria/{categoria}")]
    public async Task<IActionResult> GetMarcasPorCategoria(string categoria) =>
        this.ToActionResult(await _mediator.Send(new GetMarcasPorCategoriaQuery { Categoria = categoria }));

    [HttpGet("precios-por-categoria/{categoria}")]
    public async Task<IActionResult> GetEstadisticasPreciosPorCategoria(string categoria) =>
        this.ToActionResult(await _mediator.Send(new GetEstadisticasPreciosPorCategoriaQuery { Categoria = categoria }));

    [HttpGet("descuentos-por-categoria/{categoria}")]
    public async Task<IActionResult> GetDescuentosPorCategoria(string categoria) =>
        this.ToActionResult(await _mediator.Send(new GetDescuentosPorCategoriaQuery { Categoria = categoria }));

    [HttpGet("tallas-por-genero/{genero}")]
    public async Task<IActionResult> GetTallasPorGenero(string genero) =>
        this.ToActionResult(await _mediator.Send(new GetTallasPorGeneroQuery { Genero = genero }));

    [HttpGet("marcas-por-genero/{genero}")]
    public async Task<IActionResult> GetMarcasPorGenero(string genero) =>
        this.ToActionResult(await _mediator.Send(new GetMarcasPorGeneroQuery { Genero = genero }));

    [HttpGet("precios-por-genero/{genero}")]
    public async Task<IActionResult> GetEstadisticasPreciosPorGenero(string genero) =>
        this.ToActionResult(await _mediator.Send(new GetEstadisticasPreciosPorGeneroQuery { Genero = genero }));

    [HttpGet("descuentos-por-genero/{genero}")]
    public async Task<IActionResult> GetDescuentosPorGenero(string genero) =>
        this.ToActionResult(await _mediator.Send(new GetDescuentosPorGeneroQuery { Genero = genero }));

    [HttpGet("categorias-por-genero/{genero}")]
    public async Task<IActionResult> GetCategoriasPorGenero(string genero) =>
        this.ToActionResult(await _mediator.Send(new GetCategoriasPorGeneroQuery { Genero = genero }));

    [HttpGet("busqueda")]
    public async Task<IActionResult> Buscar(
        [FromQuery] string? nombre,
        [FromQuery] string? categoria,
        [FromQuery] string? genero) =>
        this.ToActionResult(await _mediator.Send(new BuscarPrendasConDescuentoQuery
        {
            Nombre = nombre,
            Categoria = categoria,
            Genero = genero
        }));

    [HttpGet("busqueda-por-nombre-genero")]
    public async Task<IActionResult> BuscarPorNombreYGenero(
        [FromQuery] string? nombre,
        [FromQuery] string? genero) =>
        this.ToActionResult(await _mediator.Send(new BuscarPrendasConDescuentoQuery
        {
            Nombre = nombre,
            Genero = genero
        }));

    [HttpGet("con-descuentos")]
    public async Task<IActionResult> GetDescuentosAplicados(
        [FromQuery] string? categoria,
        [FromQuery] string? genero) =>
        this.ToActionResult(await _mediator.Send(new GetDescuentosAplicadosQuery
        {
            Categoria = categoria,
            Genero = genero
        }));

    [HttpGet("con-descuentos-aleatorio/{genero}")]
    public async Task<IActionResult> GetDescuentosAplicadosAleatorio(string genero) =>
        this.ToActionResult(await _mediator.Send(new GetDescuentosAplicadosAleatorioQuery { Genero = genero }));

    [HttpGet("filtro")]
    public async Task<IActionResult> Filtrar(
        [FromQuery] string? talla,
        [FromQuery] string? categoria,
        [FromQuery] string? marca,
        [FromQuery] string? genero,
        [FromQuery] double? precioMin,
        [FromQuery] double? precioMax,
        [FromQuery] double? descMin,
        [FromQuery] double? descMax) =>
        this.ToActionResult(await _mediator.Send(new FiltrarPrendasDinamicoQuery
        {
            Talla = talla,
            Categoria = categoria,
            Marca = marca,
            Genero = genero,
            PrecioMin = precioMin,
            PrecioMax = precioMax,
            DescMin = descMin,
            DescMax = descMax
        }));
}
