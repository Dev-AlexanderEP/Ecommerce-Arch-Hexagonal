using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.Usuario.Commands;

public class DeleteUsuarioCommand : IRequest<ApiResponseDto<bool>>
{
    public required long UsuarioId { get; set; }
}

public class DeleteUsuarioCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteUsuarioCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
    {
        var repo   = _uow.Repository<UsuarioEntity>();
        var entity = await repo.GetById(request.UsuarioId);

        if (entity is null)
            return ApiResponseDto<bool>.Fail($"Usuario no encontrado para id {request.UsuarioId}.");

        await repo.Delete(request.UsuarioId);
        await _uow.Complete();

        return ApiResponseDto<bool>.Ok(true, "Usuario eliminado correctamente.");
    }
}
