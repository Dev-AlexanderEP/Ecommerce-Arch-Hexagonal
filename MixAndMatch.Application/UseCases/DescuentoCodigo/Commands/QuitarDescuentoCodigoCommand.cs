using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class QuitarDescuentoCodigoCommand : IRequest<ApiResponse<object>>
{
    public required string Codigo { get; set; }

    [JsonIgnore]
    public long SolicitanteId { get; set; }
}

public class QuitarDescuentoCodigoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<QuitarDescuentoCodigoCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(
        QuitarDescuentoCodigoCommand request,
        CancellationToken cancellationToken)
    {
        var cupon = await _uow.DescuentoCodigos.BuscarPorCodigo(request.Codigo.Trim());

        if (cupon is null)
            return ApiResponse<object>.Fail(
                $"El código '{request.Codigo}' no existe.", ErrorType.NotFound);

        var uso = await _uow.DescuentoUsuarios.BuscarPorCodigoYUsuario(cupon.Id, request.SolicitanteId);

        if (uso is null)
            return ApiResponse<object>.Fail(
                "No tienes este código de descuento aplicado.", ErrorType.NotFound);

        await _uow.DescuentoUsuarios.Delete(uso.Id);
        await _uow.Complete();

        return ApiResponse<object>.Ok(null!);
    }
}
