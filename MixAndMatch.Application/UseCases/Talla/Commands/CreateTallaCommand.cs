using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class CreateTallaCommand : IRequest<ApiResponseDto<TallaResponseDto>>
{
    public required string NomTalla { get; set; }
}

public class CreateTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateTallaCommand, ApiResponseDto<TallaResponseDto>>
{
    public async Task<ApiResponseDto<TallaResponseDto>> Handle(CreateTallaCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.NomTalla ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            return ApiResponseDto<TallaResponseDto>.Fail("El nombre de la talla es obligatorio.");

        var repo = _uow.Repository<TallaEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomTalla == nombre))
            return ApiResponseDto<TallaResponseDto>.Fail("La talla ya existe.");

        var entity = new TallaEntity { NomTalla = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponseDto<TallaResponseDto>.Ok(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
