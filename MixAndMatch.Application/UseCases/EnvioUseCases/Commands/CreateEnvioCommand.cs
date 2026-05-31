using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.Envio.Commands;

public class CreateEnvioCommand : IRequest<ApiResponseDto<EnvioResponseDto>>
{
    public required long VentaId { get; set; }

    public required long DatosEnvioId { get; set; }

    public required decimal CostoEnvio { get; set; }

    public required DateOnly FechaEnvio { get; set; }

    public DateOnly? FechaEntrega { get; set; }

    public required string Estado { get; set; }

    public required string MetodoEnvio { get; set; }

    public string? TrackingNumber { get; set; }
}

public class CreateEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateEnvioCommand, ApiResponseDto<EnvioResponseDto>>
{
    public async Task<ApiResponseDto<EnvioResponseDto>> Handle(
        CreateEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var venta = await _uow.Repository<VentaEntity>()
            .GetById(request.VentaId);

        if (venta is null)
            return ApiResponseDto<EnvioResponseDto>
                .Fail($"Venta no encontrada para id {request.VentaId}.");

        var datosEnvio = await _uow.Repository<DatosEnvioEntity>()
            .GetById(request.DatosEnvioId);

        if (datosEnvio is null)
            return ApiResponseDto<EnvioResponseDto>
                .Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.");

        var existing = (await _uow.Repository<EnvioEntity>().GetAll())
            .FirstOrDefault(x => x.VentaId == request.VentaId);

        if (existing is not null)
            return ApiResponseDto<EnvioResponseDto>
                .Fail($"La venta {request.VentaId} ya tiene un envío registrado.");

        var entity = new EnvioEntity
        {
            VentaId = request.VentaId,
            DatosEnvioId = request.DatosEnvioId,
            CostoEnvio = request.CostoEnvio,
            FechaEnvio = request.FechaEnvio,
            FechaEntrega = request.FechaEntrega,
            Estado = request.Estado,
            MetodoEnvio = request.MetodoEnvio,
            TrackingNumber = request.TrackingNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<EnvioEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponseDto<EnvioResponseDto>.Ok(new EnvioResponseDto
        {
            Id = entity.Id,
            VentaId = entity.VentaId,
            DatosEnvioId = entity.DatosEnvioId,
            CostoEnvio = entity.CostoEnvio,
            FechaEnvio = entity.FechaEnvio,
            FechaEntrega = entity.FechaEntrega,
            Estado = entity.Estado,
            MetodoEnvio = entity.MetodoEnvio,
            TrackingNumber = entity.TrackingNumber,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}