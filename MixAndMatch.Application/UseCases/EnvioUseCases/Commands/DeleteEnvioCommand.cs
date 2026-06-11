using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Commands;

public class DeleteEnvioCommand : IRequest<ApiResponse<bool>>
{
    public required long EnvioId { get; set; }
}

public class DeleteEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteEnvioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<EnvioEntity>();

        var entity = await repo.GetById(request.EnvioId);

        if (entity is null)
            return ApiResponse<bool>
                .Fail($"EnvÃ­o no encontrado para id {request.EnvioId}.");

        await repo.Delete(request.EnvioId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "EnvÃ­o eliminado correctamente.");
    }
}