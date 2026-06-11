using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Commands;

public class DeleteDatosEnvioCommand : IRequest<ApiResponse<bool>>
{
    public required long DatosEnvioId { get; set; }
}

public class DeleteDatosEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteDatosEnvioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteDatosEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DatosEnvioEntity>();

        var entity = await repo.GetById(request.DatosEnvioId);

        if (entity is null)
            return ApiResponse<bool>
                .Fail($"Datos de envÃ­o no encontrados para id {request.DatosEnvioId}.");

        await repo.Delete(request.DatosEnvioId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Datos de envÃ­o eliminados correctamente.");
    }
}