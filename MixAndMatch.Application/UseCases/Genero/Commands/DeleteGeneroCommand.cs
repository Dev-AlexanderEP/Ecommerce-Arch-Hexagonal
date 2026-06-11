using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class DeleteGeneroCommand : IRequest<ApiResponse<bool>>
{
    public required long GeneroId { get; set; }
}

public class DeleteGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteGeneroCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteGeneroCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<GeneroEntity>();
        var entity = await repo.GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"GÃ©nero no encontrado para id {request.GeneroId}.");
        }

        var prendas = await _uow.Repository<PrendaEntity>().GetAll();
        if (prendas.Any(x => x.GeneroId == request.GeneroId))
        {
            return ApiResponse<bool>.Fail("El gÃ©nero tiene prendas asociadas.");
        }

        await repo.Delete(request.GeneroId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "GÃ©nero eliminado correctamente.");
    }
}
