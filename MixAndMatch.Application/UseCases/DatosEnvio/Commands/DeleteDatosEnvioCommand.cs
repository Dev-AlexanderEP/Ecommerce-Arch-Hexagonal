using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Commands;

public class DeleteDatosEnvioCommand : IRequest<ApiResponse<bool>>
{
    public required long DatosEnvioId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class DeleteDatosEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteDatosEnvioCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteDatosEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.DatosEnvios.GetById(request.DatosEnvioId);

        if (entity is null)
            return ApiResponse<bool>
                .Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.");

        if (entity.UsuarioId != request.SolicitanteId)
            return ApiResponse<bool>
                .Fail("No tienes acceso a estos datos de envío.", ErrorType.Forbidden);

        await _uow.DatosEnvios.Delete(request.DatosEnvioId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Datos de envío eliminados correctamente.");
    }
}