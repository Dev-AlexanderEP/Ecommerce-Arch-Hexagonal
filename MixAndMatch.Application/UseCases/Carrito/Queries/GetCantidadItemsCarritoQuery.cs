using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Carrito.Queries;

public class GetCantidadItemsCarritoQuery : IRequest<ApiResponse<int>>
{
    public required long CarritoId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class GetCantidadItemsCarritoQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetCantidadItemsCarritoQuery, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(
        GetCantidadItemsCarritoQuery request,
        CancellationToken cancellationToken)
    {
        var carrito = await _uow.Carritos.GetById(request.CarritoId);
        if (carrito is null)
            return ApiResponse<int>.Fail($"Carrito no encontrado para id {request.CarritoId}.", ErrorType.NotFound);

        if (carrito.UsuarioId != request.SolicitanteId)
            return ApiResponse<int>.Fail("No tienes acceso a este carrito.", ErrorType.Forbidden);

        var cantidad = await _uow.Carritos.ContarItemsDistintos(request.CarritoId);
        return ApiResponse<int>.Ok(cantidad);
    }
}
