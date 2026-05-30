using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class CreateGeneroCommand : IRequest<ApiResponseDto<GeneroResponseDto>>
{
    public required string NomGenero { get; set; }
}

public class CreateGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateGeneroCommand, ApiResponseDto<GeneroResponseDto>>
{
    public async Task<ApiResponseDto<GeneroResponseDto>> Handle(CreateGeneroCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.NomGenero ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponseDto<GeneroResponseDto>.Fail("El nombre del género es obligatorio.");
        }

        var repo = _uow.Repository<GeneroEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomGenero == nombre))
        {
            return ApiResponseDto<GeneroResponseDto>.Fail("El género ya existe.");
        }

        var entity = new GeneroEntity { NomGenero = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponseDto<GeneroResponseDto>.Ok(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
