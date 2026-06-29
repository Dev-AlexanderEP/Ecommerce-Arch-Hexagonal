using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.CarritoItem.Queries;

public class GetCarritoItemsByCarritoQuery : IRequest<ApiResponse<List<CarritoItemDetalladoDto>>>
{
    public required long CarritoId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class GetCarritoItemsByCarritoQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetCarritoItemsByCarritoQuery, ApiResponse<List<CarritoItemDetalladoDto>>>
{
    public async Task<ApiResponse<List<CarritoItemDetalladoDto>>> Handle(
        GetCarritoItemsByCarritoQuery request,
        CancellationToken cancellationToken)
    {
        var carrito = await _uow.Carritos.GetById(request.CarritoId);
        if (carrito is null)
            return ApiResponse<List<CarritoItemDetalladoDto>>.Fail(
                $"Carrito no encontrado para id {request.CarritoId}.", ErrorType.NotFound);

        if (carrito.UsuarioId != request.SolicitanteId)
            return ApiResponse<List<CarritoItemDetalladoDto>>.Fail(
                "No tienes acceso a este carrito.", ErrorType.Forbidden);

        var items = await _uow.CarritoItems.GetDetalladosByCarritoId(request.CarritoId);

        return ApiResponse<List<CarritoItemDetalladoDto>>.Ok(
            items.Select(i =>
            {
                var prenda = i.PrendaTalla.Prenda;
                var imagenPrincipal = prenda.PrendaImagens
                    .FirstOrDefault(img => img.Tipo == TipoImagen.PRINCIPAL)?.Url;

                return new CarritoItemDetalladoDto
                {
                    Id             = i.Id,
                    CarritoId      = i.CarritoId,
                    PrendaTallaId  = i.PrendaTallaId,
                    PrendaId       = i.PrendaTalla.PrendaId,
                    TallaId        = i.PrendaTalla.TallaId,
                    PrecioUnitario = i.PrecioUnitario,
                    Cantidad       = i.Cantidad,
                    CreatedAt      = i.CreatedAt,
                    UpdatedAt      = i.UpdatedAt,
                    Prenda = new PrendaResumenCarritoDto
                    {
                        Nombre        = prenda.Nombre,
                        Precio        = prenda.Precio,
                        ImagenPrincipal = imagenPrincipal,
                        NomMarca      = prenda.Marca.NomMarca
                    },
                    Talla = new TallaResumenCarritoDto
                    {
                        NomTalla = i.PrendaTalla.Talla.NomTalla
                    }
                };
            }).ToList());
    }
}
