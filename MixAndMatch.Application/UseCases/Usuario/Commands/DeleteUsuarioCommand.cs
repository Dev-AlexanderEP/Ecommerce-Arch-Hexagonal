using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Usuario.Commands;

public class DeleteUsuarioCommand : IRequest<ApiResponse<bool>>
{
    public required long UsuarioId { get; set; }
}

public class DeleteUsuarioCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteUsuarioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Usuarios.GetById(request.UsuarioId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");
        }

        // Soft-delete: el usuario conserva su historial (ventas, resenias, etc.); solo se desactiva.
        entity.Activo = false;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Usuarios.Update(entity);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Usuario desactivado correctamente.");
    }
}
