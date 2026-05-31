using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Commands;

public class DeleteDescuentoPrendaCommand : IRequest<ApiResponseDto<bool>>
{
    public required long DescuentoPrendaId { get; set; }
}

public class DeleteDescuentoPrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteDescuentoPrendaCommand, ApiResponseDto<bool>>
{
    public async Task<ApiResponseDto<bool>> Handle(DeleteDescuentoPrendaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<DescuentoPrendaEntity>();
        var entity = await repo.GetById(request.DescuentoPrendaId);
        if (entity is null)
        {
            return ApiResponseDto<bool>.Fail($"Descuento de prenda no encontrado para id {request.DescuentoPrendaId}.");
        }

        await repo.Delete(request.DescuentoPrendaId);
        await _uow.Complete();
        return ApiResponseDto<bool>.Ok(true, "Descuento de prenda eliminado correctamente.");
    }
}
