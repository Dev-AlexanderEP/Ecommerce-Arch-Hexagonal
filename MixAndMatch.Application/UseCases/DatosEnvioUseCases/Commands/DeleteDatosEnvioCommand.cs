using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Commands;

public class DeleteDatosEnvioCommand : IRequest<ApiResponseDto<bool>>
{
    public required long DatosEnvioId { get; set; }
}

public class DeleteDatosEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteDatosEnvioCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(
        DeleteDatosEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DatosEnvioEntity>();

        var entity = await repo.GetById(request.DatosEnvioId);

        if (entity is null)
            return ApiResponseDto<bool>
                .Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.");

        await repo.Delete(request.DatosEnvioId);
        await _uow.Complete();

        return ApiResponseDto<bool>.Ok(true, "Datos de envío eliminados correctamente.");
    }
}