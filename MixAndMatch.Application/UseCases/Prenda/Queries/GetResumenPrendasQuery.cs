using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetResumenPrendasQuery : IRequest<ApiResponse<ResumenPrendasDto>> { }

public class GetResumenPrendasQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetResumenPrendasQuery, ApiResponse<ResumenPrendasDto>>
{
    public async Task<ApiResponse<ResumenPrendasDto>> Handle(GetResumenPrendasQuery request, CancellationToken ct)
        => ApiResponse<ResumenPrendasDto>.Ok(await _uow.Prendas.GetResumen());
}
