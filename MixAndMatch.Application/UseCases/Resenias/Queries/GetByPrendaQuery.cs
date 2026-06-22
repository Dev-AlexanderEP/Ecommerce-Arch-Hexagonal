using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Resenias.Queries;

public class GetByPrendaQuery : IRequest<ApiResponse<IEnumerable<ReseniaByPrendaItemDto>>>
{
    public required long PrendaId { get; set; }
}

public class GetByPrendaQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetByPrendaQuery, ApiResponse<IEnumerable<ReseniaByPrendaItemDto>>>
{
    public async Task<ApiResponse<IEnumerable<ReseniaByPrendaItemDto>>> Handle(GetByPrendaQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Resenias.GetByPrendaAsync(request.PrendaId);

        // Una prenda sin resenias no es un error: se devuelve 200 con data: [].
        return ApiResponse<IEnumerable<ReseniaByPrendaItemDto>>.Ok(items);
    }
}
