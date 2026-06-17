using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Queries;

public class GetDescuentoUsuarioByIdQuery : IRequest<ApiResponse<DescuentoUsuarioResponseDto>>
{
    public required long DescuentoUsuarioId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class GetDescuentoUsuarioByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentoUsuarioByIdQuery, ApiResponse<DescuentoUsuarioResponseDto>>
{
    public async Task<ApiResponse<DescuentoUsuarioResponseDto>> Handle(GetDescuentoUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.DescuentoUsuarios.GetById(request.DescuentoUsuarioId);
        if (entity is null)
        {
            return ApiResponse<DescuentoUsuarioResponseDto>.Fail($"Registro de uso de descuento no encontrado para id {request.DescuentoUsuarioId}.");
        }

        if (entity.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<DescuentoUsuarioResponseDto>.Fail("No tienes acceso a este registro de uso de descuento.", ErrorType.Forbidden);
        }

        return ApiResponse<DescuentoUsuarioResponseDto>.Ok(new DescuentoUsuarioResponseDto
        {
            Id = entity.Id,
            DescuentoCodigoId = entity.DescuentoCodigoId,
            UsuarioId = entity.UsuarioId,
            FechaUso = entity.FechaUso,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
