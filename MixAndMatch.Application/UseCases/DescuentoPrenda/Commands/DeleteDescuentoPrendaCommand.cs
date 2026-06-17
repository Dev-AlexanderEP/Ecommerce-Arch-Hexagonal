using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Commands;

public class DeleteDescuentoPrendaCommand : IRequest<ApiResponse<bool>>
{
    public required long DescuentoPrendaId { get; set; }
}

public class DeleteDescuentoPrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoPrendaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDescuentoPrendaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DescuentoPrendaEntity>();
        var entity = await repo.GetById(request.DescuentoPrendaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Descuento de prenda no encontrado para id {request.DescuentoPrendaId}.");
        }

        await repo.Delete(request.DescuentoPrendaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Descuento de prenda eliminado correctamente.");
    }
}
