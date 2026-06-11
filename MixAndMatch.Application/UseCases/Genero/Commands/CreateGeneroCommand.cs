using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class CreateGeneroCommand : IRequest<ApiResponse<GeneroResponseDto>>
{
    public required string NomGenero { get; set; }
}

public class CreateGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateGeneroCommand, ApiResponse<GeneroResponseDto>>
{
    public async Task<ApiResponse<GeneroResponseDto>> Handle(CreateGeneroCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.NomGenero ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<GeneroResponseDto>.Fail("El nombre del gÃ©nero es obligatorio.");
        }

        var repo = _uow.Repository<GeneroEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomGenero == nombre))
        {
            return ApiResponse<GeneroResponseDto>.Fail("El gÃ©nero ya existe.");
        }

        var entity = new GeneroEntity { NomGenero = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponse<GeneroResponseDto>.Ok(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
