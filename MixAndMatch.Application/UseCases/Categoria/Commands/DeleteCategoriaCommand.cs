using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class DeleteCategoriaCommand : IRequest<ApiResponseDto<bool>>
{
    public required long CategoriaId { get; set; }
}

public class DeleteCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteCategoriaCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CategoriaEntity>();
        var entity = await repo.GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Categoría no encontrada para id {request.CategoriaId}.");
        }

        var prendas = await _uow.Repository<PrendaEntity>().GetAll();
        if (prendas.Any(x => x.CategoriaId == request.CategoriaId))
        {
            return ApiResponseDto<bool>.Fail("La categoría tiene prendas asociadas.");
        }

        var descuentos = await _uow.Repository<DescuentoCategoriaEntity>().GetAll();
        if (descuentos.Any(x => x.CategoriaId == request.CategoriaId))
        {
            return ApiResponseDto<bool>.Fail("La categoría tiene descuentos asociados.");
        }

        await repo.Delete(request.CategoriaId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Categoría eliminada correctamente.");
    }
}
