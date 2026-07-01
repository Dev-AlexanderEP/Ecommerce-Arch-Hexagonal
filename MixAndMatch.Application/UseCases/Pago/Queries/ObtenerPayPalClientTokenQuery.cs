using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Pago.Queries;

public class ObtenerPayPalClientTokenQuery : IRequest<ApiResponse<string>>
{
}

public class ObtenerPayPalClientTokenQueryHandler(IPayPalGatewayService _gateway)
    : IRequestHandler<ObtenerPayPalClientTokenQuery, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(ObtenerPayPalClientTokenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var clientToken = await _gateway.ObtenerClientToken();
            return ApiResponse<string>.Ok(clientToken);
        }
        catch (Exception)
        {
            return ApiResponse<string>.Fail("No se pudo obtener el client token de PayPal.", ErrorType.Validation);
        }
    }
}
