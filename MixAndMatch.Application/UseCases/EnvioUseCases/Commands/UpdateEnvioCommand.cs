using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.Envio.Commands;

public class UpdateEnvioCommand : IRequest<ApiResponseDto<EnvioResponseDto>>
{
    public required long EnvioId { get; set; }

    public required long VentaId { get; set; }

    public required long DatosEnvioId { get; set; }

    public required decimal CostoEnvio { get; set; }

    public required DateOnly FechaEnvio { get; set; }

    public DateOnly? FechaEntrega { get; set; }

    public required string Estado { get; set; }

    public required string MetodoEnvio { get; set; }

    public string? TrackingNumber { get; set; }
}

public class UpdateEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdateEnvioCommand, ApiResponseDto<EnvioResponseDto>>
{
    public async Task<ApiResponseDto<EnvioResponseDto>> Handle(
        UpdateEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<EnvioEntity>()
            .GetById(request.EnvioId);

        if (entity is null)
            return ApiResponseDto<EnvioResponseDto>
                .Fail($"Envío no encontrado para id {request.EnvioId}.");

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

        entity.VentaId = request.VentaId;
        entity.DatosEnvioId = request.DatosEnvioId;
        entity.CostoEnvio = request.CostoEnvio;
        entity.FechaEnvio = request.FechaEnvio;
        entity.FechaEntrega = request.FechaEntrega;
        entity.Estado = request.Estado;
        entity.MetodoEnvio = request.MetodoEnvio;
        entity.TrackingNumber = request.TrackingNumber;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Repository<EnvioEntity>().Update(entity);
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