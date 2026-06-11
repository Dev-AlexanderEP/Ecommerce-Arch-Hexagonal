using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetEnvioByIdQuery : IRequest<ApiResponse<EnvioResponseDto>>
{
    public required long EnvioId { get; set; }
}

public class GetEnvioByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetEnvioByIdQuery, ApiResponse<EnvioResponseDto>>
{
    public async Task<ApiResponse<EnvioResponseDto>> Handle(
        GetEnvioByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<EnvioEntity>()
            .GetById(request.EnvioId);

        if (entity is null)
            return ApiResponse<EnvioResponseDto>
                .Fail($"EnvÃ­o no encontrado para id {request.EnvioId}.");

        return ApiResponse<EnvioResponseDto>.Ok(new EnvioResponseDto
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