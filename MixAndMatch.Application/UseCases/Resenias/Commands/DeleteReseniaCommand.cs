using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class DeleteReseniaCommand : IRequest<ApiResponse<bool>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long ReseniaId { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token
    public long SolicitanteId { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token
    public bool EsAdmin { get; set; }
}

public class DeleteReseniaCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteReseniaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteReseniaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Resenias.GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        // El ADMIN puede borrar cualquier resenia; el CLIENTE solo la suya.
        if (!request.EsAdmin && entity.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<bool>.Fail("No tienes acceso a esta resenia.", ErrorType.Forbidden);
        }

        await _uow.Resenias.Delete(request.ReseniaId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Resenia eliminada correctamente.");
    }
}
