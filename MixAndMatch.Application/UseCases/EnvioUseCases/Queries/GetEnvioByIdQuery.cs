using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetEnvioByIdQuery : IRequest<ApiResponseDto<EnvioResponseDto>>
{
    public required long EnvioId { get; set; }
}

public class GetEnvioByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetEnvioByIdQuery, ApiResponseDto<EnvioResponseDto>>
{
    public async Task<ApiResponseDto<EnvioResponseDto>> Handle(
        GetEnvioByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<EnvioEntity>()
            .GetById(request.EnvioId);

        if (entity is null)
            return ApiResponseDto<EnvioResponseDto>
                .Fail($"Envío no encontrado para id {request.EnvioId}.");

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