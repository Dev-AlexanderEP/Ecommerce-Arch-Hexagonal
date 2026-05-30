using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class UpdateGeneroCommand : IRequest<ApiResponseDto<GeneroResponseDto>>
{
    public required long GeneroId { get; set; }
    public required string NomGenero { get; set; }
}

public class UpdateGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateGeneroCommand, ApiResponseDto<GeneroResponseDto>>
{
    public async Task<ApiResponseDto<GeneroResponseDto>> Handle(UpdateGeneroCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<GeneroEntity>();
        var entity = await repo.GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponseDto<GeneroResponseDto>.Fail($"Género no encontrado para id {request.GeneroId}.");
        }

        var nombre = (request.NomGenero ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponseDto<GeneroResponseDto>.Fail("El nombre del género es obligatorio.");
        }

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.GeneroId && x.NomGenero == nombre))
        {
            return ApiResponseDto<GeneroResponseDto>.Fail("El género ya existe.");
        }

        entity.NomGenero = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponseDto<GeneroResponseDto>.Ok(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
