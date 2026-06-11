using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Usuario.Commands;

public class DeleteUsuarioCommand : IRequest<ApiResponse<bool>>
{
    public required long UsuarioId { get; set; }
}

public class DeleteUsuarioCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteUsuarioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
    {
        var repo   = _uow.Repository<UsuarioEntity>();
        var entity = await repo.GetById(request.UsuarioId);

        if (entity is null)
            return ApiResponse<bool>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");

        await repo.Delete(request.UsuarioId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Usuario eliminado correctamente.");
    }
}
