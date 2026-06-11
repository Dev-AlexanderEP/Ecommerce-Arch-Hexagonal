using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Queries;

public class GetAllEnviosQuery : IRequest<ApiPaginationResponse<EnvioResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllEnviosQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllEnviosQuery, ApiPaginationResponse<EnvioResponseDto>>
{
    public async Task<ApiPaginationResponse<EnvioResponseDto>> Handle(
        GetAllEnviosQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<EnvioEntity>().GetPaged(request.Page, request.PageSize);

        if (!items.Any())
            return ApiPaginationResponse<EnvioResponseDto>
                .Fail("No se encontraron envíos.");

        return ApiPaginationResponse<EnvioResponseDto>.Ok(
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
            }), total, request.Page, request.PageSize);
    }
}