using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class UpdateTallaCommand : IRequest<ApiResponseDto<TallaResponseDto>>
{
    public required long TallaId { get; set; }
    public required string NomTalla { get; set; }
}

public class UpdateTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateTallaCommand, ApiResponseDto<TallaResponseDto>>
{
    public async Task<ApiResponseDto<TallaResponseDto>> Handle(UpdateTallaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<TallaEntity>();
        var entity = await repo.GetById(request.TallaId);
        if (entity is null)
            return ApiResponseDto<TallaResponseDto>.Fail($"Talla no encontrada para id {request.TallaId}.");

        var nombre = (request.NomTalla ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            return ApiResponseDto<TallaResponseDto>.Fail("El nombre de la talla es obligatorio.");

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.TallaId && x.NomTalla == nombre))
            return ApiResponseDto<TallaResponseDto>.Fail("La talla ya existe.");

        entity.NomTalla = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponseDto<TallaResponseDto>.Ok(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
