using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Auth.Commands;

public class ChangePasswordCommand : IRequest<ApiResponse<bool>>
{
    public long UsuarioId { get; set; }
    public required string ContraseniaActual { get; set; }
    public required string ContraseniaNueva { get; set; }
}

public class ChangePasswordCommandHandler(IUnitOfWork _uow, IPasswordService _passwordService)
    : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var repo   = _uow.Repository<UsuarioEntity>();
        var entity = await repo.GetById(request.UsuarioId);

        if (entity is null || string.IsNullOrEmpty(entity.Contrasenia))
            return ApiResponse<bool>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");

        if (!_passwordService.Verify(request.ContraseniaActual, entity.Contrasenia))
            return ApiResponse<bool>.Fail("La contraseña actual es incorrecta.");

        entity.Contrasenia = _passwordService.Hash(request.ContraseniaNueva);
        entity.UpdatedAt   = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true);
    }
}
