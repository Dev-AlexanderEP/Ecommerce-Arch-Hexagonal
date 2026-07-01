using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;

namespace MixAndMatch.Application.Services;

public interface IConfirmacionPagoService
{
    Task<ApiResponse<PagoResponseDto>> ConfirmarAsync(long pagoId, CancellationToken ct = default);
}
