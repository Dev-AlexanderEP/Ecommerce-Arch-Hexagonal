using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Envio.Commands;

public class DeleteEnvioCommand : IRequest<ApiResponse<bool>>
{
    public required long EnvioId { get; set; }
}

public class DeleteEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteEnvioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteEnvioCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Envios.GetById(request.EnvioId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Envío no encontrado para id {request.EnvioId}.");
        }

        await _uow.Envios.Delete(request.EnvioId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Envío eliminado correctamente.");
    }
}
