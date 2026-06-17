using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class DeleteTallaCommand : IRequest<ApiResponse<bool>>
{
    public required long TallaId { get; set; }
}

public class DeleteTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteTallaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteTallaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Tallas.GetById(request.TallaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Talla no encontrada para id {request.TallaId}.");
        }

        if (await _uow.Tallas.TienePrendaTallas(request.TallaId))
        {
            return ApiResponse<bool>.Fail("La talla tiene prendas asociadas.", ErrorType.Conflict);
        }

        await _uow.Tallas.Delete(request.TallaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Talla eliminada correctamente.");
    }
}
