using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCodigoEntity = MixAndMatch.Domain.Entities.DescuentoCodigo;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class DeleteDescuentoCodigoCommand : IRequest<ApiResponse<bool>>
{
    public required long DescuentoCodigoId { get; set; }
}

public class DeleteDescuentoCodigoCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoCodigoCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDescuentoCodigoCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DescuentoCodigoEntity>();
        var entity = await repo.GetById(request.DescuentoCodigoId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Descuento de código no encontrado para id {request.DescuentoCodigoId}.");
        }

        await repo.Delete(request.DescuentoCodigoId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Descuento de código eliminado correctamente.");
    }
}
