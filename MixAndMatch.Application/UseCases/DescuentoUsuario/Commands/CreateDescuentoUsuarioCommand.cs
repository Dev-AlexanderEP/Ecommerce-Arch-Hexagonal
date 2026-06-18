using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoUsuarioEntity = MixAndMatch.Domain.Entities.DescuentoUsuario;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Commands;

public class CreateDescuentoUsuarioCommand : IRequest<ApiResponse<DescuentoUsuarioResponseDto>>
{
    public required long DescuentoCodigoId { get; set; }
    public required DateOnly FechaUso { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }
}

public class CreateDescuentoUsuarioCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoUsuarioCommand, ApiResponse<DescuentoUsuarioResponseDto>>
{
    public async Task<ApiResponse<DescuentoUsuarioResponseDto>> Handle(CreateDescuentoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var descuentoCodigo = await _uow.DescuentoCodigos.GetById(request.DescuentoCodigoId);
        if (descuentoCodigo is null)
        {
            return ApiResponse<DescuentoUsuarioResponseDto>.Fail($"Código de descuento no encontrado para id {request.DescuentoCodigoId}.", ErrorType.Validation);
        }

        if (await _uow.DescuentoUsuarios.ExisteParaUsuario(request.DescuentoCodigoId, request.SolicitanteId))
        {
            return ApiResponse<DescuentoUsuarioResponseDto>.Fail("Ya registraste el uso de este código de descuento.", ErrorType.Conflict);
        }

        var entity = new DescuentoUsuarioEntity
        {
            DescuentoCodigoId = request.DescuentoCodigoId,
            UsuarioId = request.SolicitanteId,
            FechaUso = request.FechaUso,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.DescuentoUsuarios.Add(entity);
        await _uow.Complete();

        return ApiResponse<DescuentoUsuarioResponseDto>.Created(new DescuentoUsuarioResponseDto
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
