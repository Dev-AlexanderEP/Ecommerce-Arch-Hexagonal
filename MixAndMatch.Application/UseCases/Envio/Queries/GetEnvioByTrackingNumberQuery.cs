using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Envio;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetEnvioByTrackingNumberQuery : IRequest<ApiResponse<EnvioTrackingResponseDto>>
{
    public required string TrackingNumber { get; set; }
}

public class GetEnvioByTrackingNumberQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetEnvioByTrackingNumberQuery, ApiResponse<EnvioTrackingResponseDto>>
{
    public async Task<ApiResponse<EnvioTrackingResponseDto>> Handle(
        GetEnvioByTrackingNumberQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.Envios.FindByTrackingNumberDetallado(request.TrackingNumber);
        if (entity is null)
            return ApiResponse<EnvioTrackingResponseDto>.Fail(
                $"Envío no encontrado para tracking number '{request.TrackingNumber}'.", ErrorType.NotFound);

        var d = entity.DatosEnvio;
        var v = entity.Venta;

        return ApiResponse<EnvioTrackingResponseDto>.Ok(new EnvioTrackingResponseDto
        {
            Id             = entity.Id,
            TrackingNumber = entity.TrackingNumber,
            Estado         = entity.Estado.ToString(),
            MetodoEnvio    = entity.MetodoEnvio,
            CostoEnvio     = entity.CostoEnvio,
            FechaEnvio     = entity.FechaEnvio,
            FechaEntrega   = entity.FechaEntrega,
            CreatedAt      = entity.CreatedAt,
            UpdatedAt      = entity.UpdatedAt,
            DatosPersonales = new DatosPersonalesDto
            {
                Id          = d.Id,
                Nombres     = d.Nombres,
                Apellidos   = d.Apellidos,
                Dni         = d.Dni,
                Departamento = d.Departamento,
                Provincia   = d.Provincia,
                Distrito    = d.Distrito,
                Calle       = d.Calle,
                Detalle     = d.Detalle,
                Telefono    = d.Telefono,
                Email       = d.Email
            },
            Venta = new VentaTrackingDto
            {
                Id            = v.Id,
                Estado        = v.Estado?.ToString(),
                FechaCreacion = v.FechaCreacion,
                Detalles      = v.VentasDetalles.Select(det =>
                {
                    var prenda = det.PrendaTalla.Prenda;
                    var imagenPrincipal = prenda.PrendaImagens
                        .FirstOrDefault(i => i.Tipo == TipoImagen.PRINCIPAL)?.Url;
                    return new DetalleTrackingDto
                    {
                        Id             = det.Id,
                        Cantidad       = det.Cantidad,
                        PrecioUnitario = det.PrecioUnitario,
                        Prenda = new PrendaTrackingDto
                        {
                            Nombre          = prenda.Nombre,
                            ImagenPrincipal = imagenPrincipal
                        },
                        Talla = new TallaTrackingDto
                        {
                            NomTalla = det.PrendaTalla.Talla.NomTalla
                        }
                    };
                }).ToList()
            }
        });
    }
}
