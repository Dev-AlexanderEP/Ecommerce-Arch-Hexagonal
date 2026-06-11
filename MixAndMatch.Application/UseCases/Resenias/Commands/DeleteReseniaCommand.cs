using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class DeleteReseniaCommand : IRequest<ApiResponse<bool>>
{
    public required long ReseniaId { get; set; }
}

public class DeleteReseniaCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<DeleteReseniaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteReseniaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ReseniaEntity>().GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        await _uow.Repository<ReseniaEntity>().Delete(request.ReseniaId);
        await _uow.Complete();

        return ApiResponse<bool>.Ok(true, "Resenia eliminada correctamente.");
    }
}
