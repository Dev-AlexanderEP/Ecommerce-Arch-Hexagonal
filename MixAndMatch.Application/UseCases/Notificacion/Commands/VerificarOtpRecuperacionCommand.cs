using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

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
        if (string.IsNullOrWhiteSpace(request.NuevaContrasenia) || request.NuevaContrasenia.Length < 6)
            return ApiResponse<bool>.Fail("La nueva contrasenia debe tener al menos 6 caracteres.");

        var repo = _uow.Repository<UsuarioEntity>();
        var usuario = (await repo.GetAll()).FirstOrDefault(u => u.Email == request.Email);

        if (usuario is null)
            return ApiResponse<bool>.Fail("No existe una cuenta registrada con ese correo.");

        var key = $"otp:recovery:{request.Email}";
        var guardado = await _cache.GetAsync(key);

        if (guardado is null || guardado != request.Codigo)
            return ApiResponse<bool>.Fail("El codigo es invalido o ya expiro.");

        await _cache.DeleteAsync(key);

        usuario.Contrasenia = _passwordService.Hash(request.NuevaContrasenia);
        usuario.UpdatedAt   = DateTime.UtcNow;
        await repo.Update(usuario);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Contrasenia actualizada correctamente.");
    }
}
