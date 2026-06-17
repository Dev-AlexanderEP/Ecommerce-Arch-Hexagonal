using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Carrito.Queries;

public class GetAllCarritosQuery : IRequest<ApiPaginationResponse<CarritoResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllCarritosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllCarritosQuery, ApiPaginationResponse<CarritoResponseDto>>
{
    public async Task<ApiPaginationResponse<CarritoResponseDto>> Handle(GetAllCarritosQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Carritos.GetPaged(request.Page, request.PageSize);

        // Una lista vacia no es un error: se devuelve 200 con data: [].
        return ApiPaginationResponse<CarritoResponseDto>.Ok(items.Select(x => new CarritoResponseDto
        {
            Id = x.Id,
            UsuarioId = x.UsuarioId,
            FechaCreacion = x.FechaCreacion,
            Estado = x.Estado?.ToString(),
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
