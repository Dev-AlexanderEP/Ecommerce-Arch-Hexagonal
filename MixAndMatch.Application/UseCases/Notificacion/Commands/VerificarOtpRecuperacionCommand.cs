using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Notificacion.Commands;

public class VerificarOtpRecuperacionCommand : IRequest<ApiResponse<bool>>
{
    public required string Email { get; set; }
    public required string Codigo { get; set; }
    public required string NuevaContrasenia { get; set; }
}

public class VerificarOtpRecuperacionCommandHandler(
    IUnitOfWork _uow,
    ICacheService _cache,
    IPasswordService _passwordService)
    : IRequestHandler<VerificarOtpRecuperacionCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(VerificarOtpRecuperacionCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByEmail(request.Email);

        if (usuario is null)
            return ApiResponse<bool>.Fail("No existe una cuenta registrada con ese correo.", ErrorType.NotFound);

        var key = $"otp:recovery:{request.Email}";
        var guardado = await _cache.GetAsync(key);

        if (guardado is null || guardado != request.Codigo)
            return ApiResponse<bool>.Fail("El código es inválido o ya expiró.", ErrorType.Validation);

        await _cache.DeleteAsync(key);

        usuario.Contrasenia = _passwordService.Hash(request.NuevaContrasenia);
        usuario.UpdatedAt   = DateTime.UtcNow;
        await _uow.Usuarios.Update(usuario);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Contraseña actualizada correctamente.");
    }
}
