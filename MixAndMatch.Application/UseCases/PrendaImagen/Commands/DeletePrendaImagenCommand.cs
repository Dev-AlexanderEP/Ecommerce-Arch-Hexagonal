using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Commands;

public class DeletePrendaImagenCommand : IRequest<ApiResponseDto<bool>>
{
    public required long PrendaImagenId { get; set; }
}

public class DeletePrendaImagenCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeletePrendaImagenCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeletePrendaImagenCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaImagenEntity>();
        var entity = await repo.GetById(request.PrendaImagenId);
        if (entity is null)
            return ApiResponseDto<bool>.Fail($"Imagen de prenda no encontrada para id {request.PrendaImagenId}.");

        await repo.Delete(request.PrendaImagenId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Imagen de prenda eliminada correctamente.");
    }
}
