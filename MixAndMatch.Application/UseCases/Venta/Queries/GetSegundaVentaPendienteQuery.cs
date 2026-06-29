using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Venta.Queries;

public class GetSegundaVentaPendienteQuery : IRequest<ApiResponse<long>>
{
    public required long UsuarioId { get; set; }
}

public class GetSegundaVentaPendienteQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetSegundaVentaPendienteQuery, ApiResponse<long>>
{
    public async Task<ApiResponse<long>> Handle(
        GetSegundaVentaPendienteQuery request,
        CancellationToken cancellationToken)
    {
        var ventaId = await _uow.Ventas.GetSegundaPendienteId(request.UsuarioId);

        if (ventaId is null)
            return ApiResponse<long>.Fail(
                $"No se encontró una segunda venta pendiente para el usuario {request.UsuarioId}.",
                ErrorType.NotFound);

        return ApiResponse<long>.Ok(ventaId.Value);
    }
}
