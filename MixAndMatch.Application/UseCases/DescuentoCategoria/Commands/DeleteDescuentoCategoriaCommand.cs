using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;

public class DeleteDescuentoCategoriaCommand : IRequest<ApiResponse<bool>>
{
    public required long DescuentoCategoriaId { get; set; }
}

public class DeleteDescuentoCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoCategoriaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDescuentoCategoriaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DescuentoCategoriaEntity>();
        var entity = await repo.GetById(request.DescuentoCategoriaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Descuento de categorÃ­a no encontrado para id {request.DescuentoCategoriaId}.");
        }

        await repo.Delete(request.DescuentoCategoriaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Descuento de categorÃ­a eliminado correctamente.");
    }
}
