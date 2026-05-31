using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using EnvioEntity = MixAndMatch.Domain.Entities.Envio;

namespace MixAndMatch.Application.UseCases.Envio.Commands;

public class DeleteEnvioCommand : IRequest<ApiResponseDto<bool>>
{
    public required long EnvioId { get; set; }
}

public class DeleteEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteEnvioCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(
        DeleteEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<EnvioEntity>();

        var entity = await repo.GetById(request.EnvioId);

        if (entity is null)
            return ApiResponseDto<bool>
                .Fail($"Envío no encontrado para id {request.EnvioId}.");

        await repo.Delete(request.EnvioId);
        await _uow.Complete();

        return ApiResponseDto<bool>.Ok(true, "Envío eliminado correctamente.");
    }
}