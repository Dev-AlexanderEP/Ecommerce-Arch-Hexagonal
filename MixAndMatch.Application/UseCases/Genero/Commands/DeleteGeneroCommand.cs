using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class DeleteGeneroCommand : IRequest<ApiResponseDto<bool>>
{
    public required long GeneroId { get; set; }
}

public class DeleteGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteGeneroCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteGeneroCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<GeneroEntity>();
        var entity = await repo.GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Género no encontrado para id {request.GeneroId}.");
        }

        var prendas = await _uow.Repository<PrendaEntity>().GetAll();
        if (prendas.Any(x => x.GeneroId == request.GeneroId))
        {
            return ApiResponseDto<bool>.Fail("El género tiene prendas asociadas.");
        }

        await repo.Delete(request.GeneroId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Género eliminado correctamente.");
    }
}
