using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetAllEnviosQuery : IRequest<ApiResponseDto<IEnumerable<EnvioResponseDto>>>
{
}

public class GetAllEnviosQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllEnviosQuery, ApiResponseDto<IEnumerable<EnvioResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<EnvioResponseDto>>> Handle(
        GetAllEnviosQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<EnvioEntity>().GetAll();

        if (!items.Any())
            return ApiResponseDto<IEnumerable<EnvioResponseDto>>
                .Fail("No se encontraron envíos.");

        return ApiResponseDto<IEnumerable<EnvioResponseDto>>.Ok(
            items.Select(entity => new EnvioResponseDto
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
            }));
    }
}