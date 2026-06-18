using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class CreateGeneroCommand : IRequest<ApiResponse<GeneroResponseDto>>
{
    public required string NomGenero { get; set; }
}

public class CreateGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateGeneroCommand, ApiResponse<GeneroResponseDto>>
{
    public async Task<ApiResponse<GeneroResponseDto>> Handle(CreateGeneroCommand request, CancellationToken cancellationToken)
    {
        var nombre = request.NomGenero.Trim();

        if (await _uow.Generos.ExisteConNombre(nombre))
        {
            return ApiResponse<GeneroResponseDto>.Fail("El género ya existe.", ErrorType.Conflict);
        }

        var entity = new GeneroEntity { NomGenero = nombre };
        await _uow.Generos.Add(entity);
        await _uow.Complete();

        return ApiResponse<GeneroResponseDto>.Created(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
