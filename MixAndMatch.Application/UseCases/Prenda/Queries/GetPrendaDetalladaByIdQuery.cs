using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Catalogo;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetPrendaDetalladaByIdQuery : IRequest<ApiResponse<PrendaDetalladaResponseDto>>
{
    public required long PrendaId { get; set; }
}

public class GetPrendaDetalladaByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetPrendaDetalladaByIdQuery, ApiResponse<PrendaDetalladaResponseDto>>
{
    public async Task<ApiResponse<PrendaDetalladaResponseDto>> Handle(
        GetPrendaDetalladaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var prenda = await _uow.Prendas.GetDetalladoById(request.PrendaId);
        if (prenda is null)
            return ApiResponse<PrendaDetalladaResponseDto>.Fail(
                $"Prenda no encontrada para id {request.PrendaId}.", ErrorType.NotFound);

        var imagenPrincipal = prenda.PrendaImagens
            .FirstOrDefault(i => i.Tipo == TipoImagen.PRINCIPAL)?.Url;

        var imagenHover = prenda.PrendaImagens
            .FirstOrDefault(i => i.Tipo == TipoImagen.HOVER)?.Url;

        var imagenExtra1 = prenda.PrendaImagens
            .FirstOrDefault(i => i.Tipo == TipoImagen.SECUNDARIA)?.Url;

        var imagenExtra2 = prenda.PrendaImagens
            .FirstOrDefault(i => i.Tipo == TipoImagen.TERCIARIA)?.Url;

        var imagenVideo = prenda.PrendaImagens
            .FirstOrDefault(i => i.Tipo == TipoImagen.DETALLE)?.Url;

        return ApiResponse<PrendaDetalladaResponseDto>.Ok(new PrendaDetalladaResponseDto
        {
            Id              = prenda.Id,
            Nombre          = prenda.Nombre,
            Descripcion     = prenda.Descripcion,
            Precio          = prenda.Precio,
            Activo          = prenda.Activo,
            CreatedAt       = prenda.CreatedAt,
            UpdatedAt       = prenda.UpdatedAt,
            ImagenPrincipal = imagenPrincipal,
            ImagenHover     = imagenHover,
            ImagenVideo     = imagenVideo,
            ImagenExtra1    = imagenExtra1,
            ImagenExtra2    = imagenExtra2,
            Marca       = new MarcaResumenDto     { Id = prenda.Marca.Id,     NomMarca     = prenda.Marca.NomMarca },
            Categoria   = new CategoriaResumenDto { Id = prenda.Categoria.Id, NomCategoria = prenda.Categoria.NomCategoria },
            Proveedor   = new ProveedorResumenDto  { Id = prenda.Proveedor.Id, NomProveedor = prenda.Proveedor.NomProveedor },
            Tallas      = prenda.PrendaTallas.Select(pt => new TallaStockDto
            {
                PrendaTallaId = pt.Id,
                TallaId       = pt.TallaId,
                NomTalla      = pt.Talla.NomTalla,
                Stock         = pt.Stock
            }).ToList()
        });
    }
}
