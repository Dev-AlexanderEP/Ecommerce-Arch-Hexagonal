using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;
using VentaEntity = MixAndMatch.Domain.Entities.Venta;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetAllVentasQuery : IRequest<ApiResponseDto<IEnumerable<VentaResponseDto>>>
{
}

public class GetAllVentasQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllVentasQuery, ApiResponseDto<IEnumerable<VentaResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<VentaResponseDto>>> Handle(GetAllVentasQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<VentaEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<VentaResponseDto>>.Fail("No se encontraron ventas.");
        }

        return ApiResponseDto<IEnumerable<VentaResponseDto>>.Ok(items.Select(x => new VentaResponseDto
        {
            Id = x.Id,
            UsuarioId = x.UsuarioId,
            FechaCreacion = x.FechaCreacion,
            Estado = x.Estado,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
