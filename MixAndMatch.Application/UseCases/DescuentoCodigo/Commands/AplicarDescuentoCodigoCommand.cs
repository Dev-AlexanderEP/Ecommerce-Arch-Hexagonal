using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoUsuarioEntity = MixAndMatch.Domain.Entities.DescuentoUsuario;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class AplicarDescuentoCodigoCommand : IRequest<ApiResponse<DescuentoCodigoResponseDto>>
{
    public required string Codigo { get; set; }

    [JsonIgnore]
    public long SolicitanteId { get; set; }
}

public class AplicarDescuentoCodigoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<AplicarDescuentoCodigoCommand, ApiResponse<DescuentoCodigoResponseDto>>
{
    public async Task<ApiResponse<DescuentoCodigoResponseDto>> Handle(
        AplicarDescuentoCodigoCommand request,
        CancellationToken cancellationToken)
    {
        var cupon = await _uow.DescuentoCodigos.BuscarPorCodigo(request.Codigo.Trim());

        if (cupon is null)
            return ApiResponse<DescuentoCodigoResponseDto>.Fail(
                $"El código '{request.Codigo}' no existe.", ErrorType.NotFound);

        if (!cupon.Activo)
            return ApiResponse<DescuentoCodigoResponseDto>.Fail(
                "El código de descuento no está activo.", ErrorType.Validation);

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        if (cupon.FechaInicio > hoy || (cupon.FechaFin.HasValue && cupon.FechaFin < hoy))
            return ApiResponse<DescuentoCodigoResponseDto>.Fail(
                "El código de descuento está fuera del período de vigencia.", ErrorType.Validation);

        if (await _uow.DescuentoUsuarios.ExisteParaUsuario(cupon.Id, request.SolicitanteId))
            return ApiResponse<DescuentoCodigoResponseDto>.Fail(
                "Ya has utilizado este código de descuento.", ErrorType.Conflict);

        var uso = new DescuentoUsuarioEntity
        {
            DescuentoCodigoId = cupon.Id,
            UsuarioId = request.SolicitanteId,
            FechaUso = hoy,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.DescuentoUsuarios.Add(uso);
        await _uow.Complete();

        return ApiResponse<DescuentoCodigoResponseDto>.Ok(new DescuentoCodigoResponseDto
        {
            Id          = cupon.Id,
            Codigo      = cupon.Codigo,
            Descripcion = cupon.Descripcion,
            Porcentaje  = cupon.Porcentaje,
            FechaInicio = cupon.FechaInicio,
            FechaFin    = cupon.FechaFin,
            UsoMaximo   = cupon.UsoMaximo,
            Activo      = cupon.Activo,
            CreatedAt   = cupon.CreatedAt,
            UpdatedAt   = cupon.UpdatedAt
        });
    }
}
