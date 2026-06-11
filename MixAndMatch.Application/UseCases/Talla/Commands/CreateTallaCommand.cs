using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class CreateTallaCommand : IRequest<ApiResponse<TallaResponseDto>>
{
    public required string NomTalla { get; set; }
}

public class CreateTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateTallaCommand, ApiResponse<TallaResponseDto>>
{
    public async Task<ApiResponse<TallaResponseDto>> Handle(CreateTallaCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.NomTalla ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            return ApiResponse<TallaResponseDto>.Fail("El nombre de la talla es obligatorio.");

        var repo = _uow.Repository<TallaEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomTalla == nombre))
            return ApiResponse<TallaResponseDto>.Fail("La talla ya existe.");

        var entity = new TallaEntity { NomTalla = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponse<TallaResponseDto>.Ok(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
