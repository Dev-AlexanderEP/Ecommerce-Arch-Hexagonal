using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCodigoEntity = MixAndMatch.Domain.Entities.DescuentoCodigo;
using DescuentoUsuarioEntity = MixAndMatch.Domain.Entities.DescuentoUsuario;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Commands;

public class CreateDescuentoUsuarioCommand : IRequest<ApiResponseDto<DescuentoUsuarioResponseDto>>
{
    public required long DescuentoCodigoId { get; set; }
    public required long UsuarioId { get; set; }
    public required DateOnly FechaUso { get; set; }
}

public class CreateDescuentoUsuarioCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoUsuarioCommand, ApiResponseDto<DescuentoUsuarioResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoUsuarioResponseDto>> Handle(CreateDescuentoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var descuentoCodigo = await _uow.Repository<DescuentoCodigoEntity>().GetById(request.DescuentoCodigoId);
        if (descuentoCodigo is null)
        {
            return ApiResponseDto<DescuentoUsuarioResponseDto>.Fail($"Descuento de código no encontrado para id {request.DescuentoCodigoId}.");
        }

        var usuario = await _uow.Repository<UsuarioEntity>().GetById(request.UsuarioId);
        if (usuario is null)
        {
            return ApiResponseDto<DescuentoUsuarioResponseDto>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        var entity = new DescuentoUsuarioEntity
        {
            DescuentoCodigoId = request.DescuentoCodigoId,
            UsuarioId = request.UsuarioId,
            FechaUso = request.FechaUso,
            CreatedAt = DateTime.UtcNow
        };

        var repo = _uow.Repository<DescuentoUsuarioEntity>();
        await repo.Add(entity);
        await _uow.Complete();

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
