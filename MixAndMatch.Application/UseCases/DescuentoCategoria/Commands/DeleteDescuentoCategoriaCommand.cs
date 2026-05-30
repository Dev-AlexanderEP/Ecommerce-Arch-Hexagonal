using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;

public class DeleteDescuentoCategoriaCommand : IRequest<ApiResponseDto<bool>>
{
    public required long DescuentoCategoriaId { get; set; }
}

public class DeleteDescuentoCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoCategoriaCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteDescuentoCategoriaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DescuentoCategoriaEntity>();
        var entity = await repo.GetById(request.DescuentoCategoriaId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Descuento de categoría no encontrado para id {request.DescuentoCategoriaId}.");
        }

        await repo.Delete(request.DescuentoCategoriaId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Descuento de categoría eliminado correctamente.");
    }
}
