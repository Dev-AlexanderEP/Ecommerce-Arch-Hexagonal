using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class UpdateTallaCommand : IRequest<ApiResponse<TallaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long TallaId { get; set; }
    public required string NomTalla { get; set; }
}

public class UpdateTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateTallaCommand, ApiResponse<TallaResponseDto>>
{
    public async Task<ApiResponse<TallaResponseDto>> Handle(UpdateTallaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Tallas.GetById(request.TallaId);
        if (entity is null)
        {
            return ApiResponse<TallaResponseDto>.Fail($"Talla no encontrada para id {request.TallaId}.");
        }

        var nombre = request.NomTalla.Trim();
        if (await _uow.Tallas.ExisteConNombre(nombre, request.TallaId))
        {
            return ApiResponse<TallaResponseDto>.Fail("La talla ya existe.", ErrorType.Conflict);
        }

        entity.NomTalla = nombre;
        await _uow.Tallas.Update(entity);
        await _uow.Complete();

        return ApiResponse<TallaResponseDto>.Ok(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
