using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Domain.Ports.IRepositories;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Resenias.Commands;

public class DeleteReseniaCommand : IRequest<ApiResponseDto<bool>>
{
    public required long ReseniaId { get; set; }
}

public class DeleteReseniaCommandHandler(IReseniaRepository _reseniaRepository, IUnitOfWork _uow)
    : IRequestHandler<DeleteReseniaCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteReseniaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<ReseniaEntity>().GetById(request.ReseniaId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Resenia no encontrada para id {request.ReseniaId}.");
        }

        await _reseniaRepository.DeleteAsync(request.ReseniaId);
        await _uow.Complete();

        return ApiResponseDto<bool>.Ok(true, "Resenia eliminada correctamente.");
    }
}
