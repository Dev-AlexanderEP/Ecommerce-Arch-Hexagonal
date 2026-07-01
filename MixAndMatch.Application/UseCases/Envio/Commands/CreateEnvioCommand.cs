using System.Text;
using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Commands;

public class CreateEnvioCommand : IRequest<ApiResponse<EnvioResponseDto>>
{
    public required long VentaId { get; set; }
    public required long DatosEnvioId { get; set; }
    public required decimal CostoEnvio { get; set; }
    public required DateOnly FechaEnvio { get; set; }
    public DateOnly? FechaEntrega { get; set; }
    public required string Estado { get; set; }
    public required string MetodoEnvio { get; set; }
    public string? TrackingNumber { get; set; }

    [JsonIgnore] public long SolicitanteId { get; set; }
}

public class CreateEnvioCommandHandler(
    IUnitOfWork _uow,
    IEmailService _email,
    IEmailTemplateService _templates)
    : IRequestHandler<CreateEnvioCommand, ApiResponse<EnvioResponseDto>>
{
    public async Task<ApiResponse<EnvioResponseDto>> Handle(CreateEnvioCommand request, CancellationToken cancellationToken)
    {
        var venta = await _uow.Ventas.GetById(request.VentaId);
        if (venta is null)
            return ApiResponse<EnvioResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.", ErrorType.Validation);

        if (venta.UsuarioId != request.SolicitanteId)
            return ApiResponse<EnvioResponseDto>.Fail("No tienes acceso a esta venta.", ErrorType.Forbidden);

        var datosEnvio = await _uow.DatosEnvios.GetById(request.DatosEnvioId);
        if (datosEnvio is null)
            return ApiResponse<EnvioResponseDto>.Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.", ErrorType.Validation);

        if (await _uow.Envios.ExisteParaVenta(request.VentaId))
            return ApiResponse<EnvioResponseDto>.Fail($"La venta {request.VentaId} ya tiene un envío registrado.", ErrorType.Conflict);

        if (!Enum.TryParse<EstadoEnvio>(request.Estado, ignoreCase: true, out var estadoEnvio))
            return ApiResponse<EnvioResponseDto>.Fail($"Estado inválido: {request.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoEnvio>())}.", ErrorType.Validation);

        var entity = new EnvioEntity
        {
            VentaId      = request.VentaId,
            DatosEnvioId = request.DatosEnvioId,
            CostoEnvio   = request.CostoEnvio,
            FechaEnvio   = request.FechaEnvio,
            FechaEntrega = request.FechaEntrega,
            Estado       = estadoEnvio,
            MetodoEnvio  = request.MetodoEnvio,
            TrackingNumber = request.TrackingNumber,
            CreatedAt    = DateTime.UtcNow
        };

        await _uow.Envios.Add(entity);
        await _uow.Complete();

        _ = EnviarEmailNotificacionAsync(entity.Id, request, datosEnvio.Email, datosEnvio.Nombres);

        return ApiResponse<EnvioResponseDto>.Created(new EnvioResponseDto
        {
            Id           = entity.Id,
            VentaId      = entity.VentaId,
            DatosEnvioId = entity.DatosEnvioId,
            CostoEnvio   = entity.CostoEnvio,
            FechaEnvio   = entity.FechaEnvio,
            FechaEntrega = entity.FechaEntrega,
            Estado       = entity.Estado.ToString(),
            MetodoEnvio  = entity.MetodoEnvio,
            TrackingNumber = entity.TrackingNumber,
            CreatedAt    = entity.CreatedAt,
            UpdatedAt    = entity.UpdatedAt
        });
    }

    private async Task EnviarEmailNotificacionAsync(
        long envioId, CreateEnvioCommand request, string email, string nombres)
    {
        try
        {
            var ventaConDetalles = await _uow.Ventas.GetByIdConDetalles(request.VentaId);
            if (ventaConDetalles is null) return;

            var filas = new StringBuilder();
            decimal total = 0;

            foreach (var det in ventaConDetalles.VentasDetalles)
            {
                var nombre = det.PrendaTalla?.Prenda?.Nombre ?? "—";
                var talla  = det.PrendaTalla?.Talla?.NomTalla ?? "—";
                var subtotal = det.Cantidad * det.PrecioUnitario;
                total += subtotal;

                filas.Append($"""
                    <tr>
                      <td style="color:#1a1a2e;font-size:14px;padding:10px 0;border-bottom:1px solid #f3f4f6;">{nombre}</td>
                      <td style="color:#1a1a2e;font-size:14px;padding:10px 0;text-align:center;border-bottom:1px solid #f3f4f6;">{talla}</td>
                      <td style="color:#1a1a2e;font-size:14px;padding:10px 0;text-align:center;border-bottom:1px solid #f3f4f6;">{det.Cantidad}</td>
                      <td style="color:#1a1a2e;font-size:14px;font-weight:bold;padding:10px 0;text-align:right;border-bottom:1px solid #f3f4f6;">S/ {subtotal:0.00}</td>
                    </tr>
                    """);
            }

            var d = await _uow.DatosEnvios.GetById(request.DatosEnvioId);
            var direccion = d is not null
                ? $"{d.Calle}, {d.Distrito}, {d.Provincia}"
                : "—";

            var html = _templates.Render("NotificacionEnvio", new Dictionary<string, string>
            {
                ["Nombres"]        = nombres,
                ["TrackingNumber"] = request.TrackingNumber ?? $"ENV-{envioId:D6}",
                ["MetodoEnvio"]    = request.MetodoEnvio,
                ["FechaEnvio"]     = request.FechaEnvio.ToString("dd/MM/yyyy"),
                ["DireccionEnvio"] = direccion,
                ["FilasProductos"] = filas.ToString(),
                ["Total"]          = total.ToString("0.00"),
            });

            await _email.SendAsync(email, "Tu pedido Mix&Match está en camino 🚚", html);
        }
        catch
        {
            // El email falla silenciosamente — el envío ya fue creado.
        }
    }
}
