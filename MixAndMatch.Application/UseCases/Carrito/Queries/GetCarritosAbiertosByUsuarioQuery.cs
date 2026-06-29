using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Carrito.Queries;

public class GetCarritosAbiertosByUsuarioQuery : IRequest<ApiResponse<List<CarritoResponseDto>>>
{
    public required long UsuarioId { get; set; }
}

public class GetCarritosAbiertosByUsuarioQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetCarritosAbiertosByUsuarioQuery, ApiResponse<List<CarritoResponseDto>>>
{
    public async Task<ApiResponse<List<CarritoResponseDto>>> Handle(
        GetCarritosAbiertosByUsuarioQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _uow.Carritos.BuscarAbiertosPorUsuarioId(request.UsuarioId);

        return ApiResponse<List<CarritoResponseDto>>.Ok(
            items.Select(e => new CarritoResponseDto
            {
                Id           = e.Id,
                UsuarioId    = e.UsuarioId,
                FechaCreacion = e.FechaCreacion,
                Estado       = e.Estado?.ToString(),
                UpdatedAt    = e.UpdatedAt
            }).ToList());
    }
}
