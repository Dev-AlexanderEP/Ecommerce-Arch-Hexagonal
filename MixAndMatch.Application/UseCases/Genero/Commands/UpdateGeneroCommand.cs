using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class UpdateGeneroCommand : IRequest<ApiResponse<GeneroResponseDto>>
{
    public required long GeneroId { get; set; }
    public required string NomGenero { get; set; }
}

public class UpdateGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateGeneroCommand, ApiResponse<GeneroResponseDto>>
{
    public async Task<ApiResponse<GeneroResponseDto>> Handle(UpdateGeneroCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<GeneroEntity>();
        var entity = await repo.GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponse<GeneroResponseDto>.Fail($"GÃ©nero no encontrado para id {request.GeneroId}.");
        }

        var nombre = (request.NomGenero ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<GeneroResponseDto>.Fail("El nombre del gÃ©nero es obligatorio.");
        }

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.GeneroId && x.NomGenero == nombre))
        {
            return ApiResponse<GeneroResponseDto>.Fail("El gÃ©nero ya existe.");
        }

        entity.NomGenero = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<GeneroResponseDto>.Ok(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
