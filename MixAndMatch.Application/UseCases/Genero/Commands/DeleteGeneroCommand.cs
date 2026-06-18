using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class DeleteGeneroCommand : IRequest<ApiResponse<bool>>
{
    public required long GeneroId { get; set; }
}

public class DeleteGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteGeneroCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteGeneroCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Generos.GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Género no encontrado para id {request.GeneroId}.");
        }

        if (await _uow.Generos.TienePrendas(request.GeneroId))
        {
            return ApiResponse<bool>.Fail("El género tiene prendas asociadas.", ErrorType.Conflict);
        }

        await _uow.Generos.Delete(request.GeneroId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Género eliminado correctamente.");
    }
}
