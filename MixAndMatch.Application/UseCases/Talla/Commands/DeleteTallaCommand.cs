using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class DeleteTallaCommand : IRequest<ApiResponse<bool>>
{
    public required long TallaId { get; set; }
}

public class DeleteTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteTallaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteTallaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<TallaEntity>();
        var entity = await repo.GetById(request.TallaId);
        if (entity is null)
            return ApiResponse<bool>.Fail($"Talla no encontrada para id {request.TallaId}.");

        var prendaTallas = await _uow.Repository<PrendaTallaEntity>().GetAll();
        if (prendaTallas.Any(x => x.TallaId == request.TallaId))
            return ApiResponse<bool>.Fail("La talla tiene prendas asociadas.");

        await repo.Delete(request.TallaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Talla eliminada correctamente.");
    }
}
