using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class DeleteCategoriaCommand : IRequest<ApiResponse<bool>>
{
    public required long CategoriaId { get; set; }
}

public class DeleteCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteCategoriaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CategoriaEntity>();
        var entity = await repo.GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"CategorÃ­a no encontrada para id {request.CategoriaId}.");
        }

        var prendas = await _uow.Repository<PrendaEntity>().GetAll();
        if (prendas.Any(x => x.CategoriaId == request.CategoriaId))
        {
            return ApiResponse<bool>.Fail("La categorÃ­a tiene prendas asociadas.");
        }

        var descuentos = await _uow.Repository<DescuentoCategoriaEntity>().GetAll();
        if (descuentos.Any(x => x.CategoriaId == request.CategoriaId))
        {
            return ApiResponse<bool>.Fail("La categorÃ­a tiene descuentos asociados.");
        }

        await repo.Delete(request.CategoriaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "CategorÃ­a eliminada correctamente.");
    }
}
