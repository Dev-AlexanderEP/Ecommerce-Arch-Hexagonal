using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;
using PrendaImagenEntity = MixAndMatch.Domain.Entities.PrendaImagen;
using PrendaTallaEntity = MixAndMatch.Domain.Entities.PrendaTalla;
using ReseniaEntity = MixAndMatch.Domain.Entities.Resenia;

namespace MixAndMatch.Application.UseCases.Prenda.Commands;

public class DeletePrendaCommand : IRequest<ApiResponseDto<bool>>
{
    public required long PrendaId { get; set; }
}

public class DeletePrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeletePrendaCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeletePrendaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaEntity>();
        var entity = await repo.GetById(request.PrendaId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Prenda no encontrada para id {request.PrendaId}.");
        }

        if ((await _uow.Repository<DescuentoPrendaEntity>().GetAll()).Any(x => x.PrendaId == request.PrendaId))
        {
            return ApiResponseDto<bool>.Fail("La prenda tiene descuentos asociados.");
        }

        if ((await _uow.Repository<PrendaImagenEntity>().GetAll()).Any(x => x.PrendaId == request.PrendaId))
        {
            return ApiResponseDto<bool>.Fail("La prenda tiene imágenes asociadas.");
        }

        if ((await _uow.Repository<PrendaTallaEntity>().GetAll()).Any(x => x.PrendaId == request.PrendaId))
        {
            return ApiResponseDto<bool>.Fail("La prenda tiene tallas asociadas.");
        }

        if ((await _uow.Repository<ReseniaEntity>().GetAll()).Any(x => x.PrendaId == request.PrendaId))
        {
            return ApiResponseDto<bool>.Fail("La prenda tiene reseñas asociadas.");
        }

        await repo.Delete(request.PrendaId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Prenda eliminada correctamente.");
    }
}
