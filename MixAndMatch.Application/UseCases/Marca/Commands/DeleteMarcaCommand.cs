using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Marca.Commands;

public class DeleteMarcaCommand : IRequest<ApiResponse<bool>>
{
    public required long MarcaId { get; set; }
}

public class DeleteMarcaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteMarcaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteMarcaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Marcas.GetById(request.MarcaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Marca no encontrada para id {request.MarcaId}.");
        }

        if (await _uow.Marcas.TienePrendas(request.MarcaId))
        {
            return ApiResponse<bool>.Fail("La marca tiene prendas asociadas.", ErrorType.Conflict);
        }

        await _uow.Marcas.Delete(request.MarcaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Marca eliminada correctamente.");
    }
}
