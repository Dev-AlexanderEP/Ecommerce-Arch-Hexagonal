using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Auth.Commands;

public class ChangePasswordCommand : IRequest<ApiResponse<bool>>
{
    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long UsuarioId { get; set; }
    public required string ContraseniaActual { get; set; }
    public required string ContraseniaNueva { get; set; }
}

public class ChangePasswordCommandHandler(IUsuarioRepository _usuarios, IUnitOfWork _uow, IPasswordService _passwordService)
    : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var entity = await _usuarios.GetById(request.UsuarioId);

        if (entity is null)
            return ApiResponse<bool>.Fail($"Usuario no encontrado para id {request.UsuarioId}.", ErrorType.NotFound);

        if (string.IsNullOrEmpty(entity.Contrasenia))
            return ApiResponse<bool>.Fail(
                "Tu cuenta inicia sesión con Google y no tiene contraseña local.", ErrorType.Validation);

        if (!_passwordService.Verify(request.ContraseniaActual, entity.Contrasenia))
            return ApiResponse<bool>.Fail("La contraseña actual es incorrecta.", ErrorType.Validation);

        entity.Contrasenia = _passwordService.Hash(request.ContraseniaNueva);
        entity.UpdatedAt   = DateTime.UtcNow;

        await _usuarios.Update(entity);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true);
    }
}
