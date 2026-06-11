using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Commands;

public class DeletePrendaImagenCommand : IRequest<ApiResponse<bool>>
{
    public required long PrendaImagenId { get; set; }
}

public class DeletePrendaImagenCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeletePrendaImagenCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeletePrendaImagenCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaImagenEntity>();
        var entity = await repo.GetById(request.PrendaImagenId);
        if (entity is null)
            return ApiResponse<bool>.Fail($"Imagen de prenda no encontrada para id {request.PrendaImagenId}.");

        await repo.Delete(request.PrendaImagenId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Imagen de prenda eliminada correctamente.");
    }
}
