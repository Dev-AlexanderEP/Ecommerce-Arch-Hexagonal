using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;
using VentasDetalleEntity = MixAndMatch.Domain.Entities.VentasDetalle;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Commands;

public class CreateVentasDetalleCommand : IRequest<ApiResponseDto<VentasDetalleResponseDto>>
{
    public required long VentaId { get; set; }
    public required long PrendaTallaId { get; set; }
    public required int Cantidad { get; set; }
    public required decimal PrecioUnitario { get; set; }
}

public class CreateVentasDetalleCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateVentasDetalleCommand, ApiResponseDto<VentasDetalleResponseDto>>
{
    public async Task<ApiResponseDto<VentasDetalleResponseDto>> Handle(CreateVentasDetalleCommand request, CancellationToken cancellationToken)
    {
        var venta = await _uow.Repository<VentaEntity>().GetById(request.VentaId);
        if (venta is null)
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail($"Venta no encontrada para id {request.VentaId}.");
        }

        var prendaTalla = await _uow.Repository<PrendaTallaEntity>().GetById(request.PrendaTallaId);
        if (prendaTalla is null)
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail($"PrendaTalla no encontrada para id {request.PrendaTallaId}.");
        }

        if (request.Cantidad <= 0)
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail("La cantidad debe ser mayor a 0.");
        }

        if (request.PrecioUnitario <= 0)
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail("El precio unitario debe ser mayor a 0.");
        }

        var detalles = await _uow.Repository<VentasDetalleEntity>().GetAll();
        if (detalles.Any(x => x.VentaId == request.VentaId && x.PrendaTallaId == request.PrendaTallaId))
        {
            return ApiResponseDto<VentasDetalleResponseDto>.Fail("Ya existe un detalle con esa combinación de VentaId y PrendaTallaId.");
        }

        var entity = new VentasDetalleEntity
        {
            VentaId = request.VentaId,
            PrendaTallaId = request.PrendaTallaId,
            Cantidad = request.Cantidad,
            PrecioUnitario = request.PrecioUnitario,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<VentasDetalleEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponseDto<VentasDetalleResponseDto>.Ok(new VentasDetalleResponseDto
        {
            Id = entity.Id,
            VentaId = entity.VentaId,
            PrendaTallaId = entity.PrendaTallaId,
            Cantidad = entity.Cantidad,
            PrecioUnitario = entity.PrecioUnitario,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
