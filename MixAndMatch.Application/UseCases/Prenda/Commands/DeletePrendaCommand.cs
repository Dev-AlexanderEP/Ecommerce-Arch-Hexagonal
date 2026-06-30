using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Prenda.Commands;

public class DeletePrendaCommand : IRequest<ApiResponse<bool>>
{
    public required long PrendaId { get; set; }
}

public class DeletePrendaCommandHandler(IUnitOfWork _uow, IStorageService _storage)
    : IRequestHandler<DeletePrendaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeletePrendaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Prendas.GetById(request.PrendaId);
        if (entity is null)
            return ApiResponse<bool>.Fail($"Prenda no encontrada para id {request.PrendaId}.");

        if (await _uow.Prendas.TieneDescuentos(request.PrendaId))
            return ApiResponse<bool>.Fail("La prenda tiene descuentos asociados.", ErrorType.Conflict);

        if (await _uow.Prendas.TieneTallas(request.PrendaId))
            return ApiResponse<bool>.Fail("La prenda tiene tallas asociadas.", ErrorType.Conflict);

        if (await _uow.Prendas.TieneResenias(request.PrendaId))
            return ApiResponse<bool>.Fail("La prenda tiene reseñas asociadas.", ErrorType.Conflict);

        // Elimina todas las imágenes del bucket R2 y luego sus registros en BD
        await _storage.DeleteByPrefixAsync($"prendas/{request.PrendaId}/");
        var imagenesRepo = _uow.Repository<MixAndMatch.Domain.Entities.PrendaImagen>();
        var imagenes = await imagenesRepo.GetAll();
        foreach (var img in imagenes.Where(i => i.PrendaId == request.PrendaId))
            await imagenesRepo.Delete(img.Id);

        await _uow.Prendas.Delete(request.PrendaId);
        await _uow.Complete();
        return ApiResponse<bool>.Ok(true, "Prenda eliminada correctamente.");
    }
}
