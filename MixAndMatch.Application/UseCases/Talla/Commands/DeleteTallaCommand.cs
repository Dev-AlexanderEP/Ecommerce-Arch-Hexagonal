using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class DeleteTallaCommand : IRequest<ApiResponseDto<bool>>
{
    public required long TallaId { get; set; }
}

public class DeleteTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteTallaCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteTallaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<TallaEntity>();
        var entity = await repo.GetById(request.TallaId);
        if (entity is null)
            return ApiResponseDto<bool>.Fail($"Talla no encontrada para id {request.TallaId}.");

        var prendaTallas = await _uow.Repository<PrendaTallaEntity>().GetAll();
        if (prendaTallas.Any(x => x.TallaId == request.TallaId))
            return ApiResponseDto<bool>.Fail("La talla tiene prendas asociadas.");

        await repo.Delete(request.TallaId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Talla eliminada correctamente.");
    }
}
