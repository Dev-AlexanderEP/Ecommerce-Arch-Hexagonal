using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class DeleteCategoriaCommand : IRequest<ApiResponse<bool>>
{
    public required long CategoriaId { get; set; }
}

public class DeleteCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteCategoriaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Categorias.GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Categoría no encontrada para id {request.CategoriaId}.");
        }

        if (await _uow.Categorias.TienePrendas(request.CategoriaId))
        {
            return ApiResponse<bool>.Fail("La categoría tiene prendas asociadas.", ErrorType.Conflict);
        }

        if (await _uow.Categorias.TieneDescuentos(request.CategoriaId))
        {
            return ApiResponse<bool>.Fail("La categoría tiene descuentos asociados.", ErrorType.Conflict);
        }

        await _uow.Categorias.Delete(request.CategoriaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Categoría eliminada correctamente.");
    }
}
