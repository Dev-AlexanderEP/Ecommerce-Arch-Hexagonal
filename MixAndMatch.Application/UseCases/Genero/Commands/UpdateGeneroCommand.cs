using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Genero.Commands;

public class UpdateGeneroCommand : IRequest<ApiResponse<GeneroResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long GeneroId { get; set; }
    public required string NomGenero { get; set; }
}

public class UpdateGeneroCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateGeneroCommand, ApiResponse<GeneroResponseDto>>
{
    public async Task<ApiResponse<GeneroResponseDto>> Handle(UpdateGeneroCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Generos.GetById(request.GeneroId);
        if (entity is null)
        {
            return ApiResponse<GeneroResponseDto>.Fail($"Género no encontrado para id {request.GeneroId}.");
        }

        var nombre = request.NomGenero.Trim();
        if (await _uow.Generos.ExisteConNombre(nombre, request.GeneroId))
        {
            return ApiResponse<GeneroResponseDto>.Fail("El género ya existe.", ErrorType.Conflict);
        }

        entity.NomGenero = nombre;
        await _uow.Generos.Update(entity);
        await _uow.Complete();

        return ApiResponse<GeneroResponseDto>.Ok(new GeneroResponseDto
        {
            Id = entity.Id,
            NomGenero = entity.NomGenero
        });
    }
}
