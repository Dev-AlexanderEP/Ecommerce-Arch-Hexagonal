using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoUsuarioEntity = MixAndMatch.Domain.Entities.DescuentoUsuario;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Queries;

public class GetDescuentoUsuarioByIdQuery : IRequest<ApiResponseDto<DescuentoUsuarioResponseDto>>
{
    public required long DescuentoUsuarioId { get; set; }
}

public class GetDescuentoUsuarioByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentoUsuarioByIdQuery, ApiResponseDto<DescuentoUsuarioResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoUsuarioResponseDto>> Handle(GetDescuentoUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DescuentoUsuarioEntity>().GetById(request.DescuentoUsuarioId);
        if (entity is null)
        {
            return ApiResponseDto<DescuentoUsuarioResponseDto>.Fail($"Registro de uso de descuento no encontrado para id {request.DescuentoUsuarioId}.");
        }

        return ApiResponseDto<DescuentoUsuarioResponseDto>.Ok(new DescuentoUsuarioResponseDto
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
