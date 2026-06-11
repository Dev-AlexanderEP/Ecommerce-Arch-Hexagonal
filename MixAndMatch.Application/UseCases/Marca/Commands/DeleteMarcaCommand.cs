using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Marca.Commands;

public class DeleteMarcaCommand : IRequest<ApiResponse<bool>>
{
    public required long MarcaId { get; set; }
}

public class DeleteMarcaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteMarcaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteMarcaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<MarcaEntity>();
        var entity = await repo.GetById(request.MarcaId);
        if (entity is null)
        {
            return ApiResponse<bool>.Fail($"Marca no encontrada para id {request.MarcaId}.");
        }

        var prendas = await _uow.Repository<PrendaEntity>().GetAll();
        if (prendas.Any(x => x.MarcaId == request.MarcaId))
        {
            return ApiResponse<bool>.Fail("La marca tiene prendas asociadas.");
        }

        await repo.Delete(request.MarcaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Marca eliminada correctamente.");
    }
}
