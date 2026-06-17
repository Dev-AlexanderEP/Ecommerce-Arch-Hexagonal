using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Marca.Commands;

public class UpdateMarcaCommand : IRequest<ApiResponse<MarcaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long MarcaId { get; set; }
    public required string NomMarca { get; set; }
}

public class UpdateMarcaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateMarcaCommand, ApiResponse<MarcaResponseDto>>
{
    public async Task<ApiResponse<MarcaResponseDto>> Handle(UpdateMarcaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Marcas.GetById(request.MarcaId);
        if (entity is null)
        {
            return ApiResponse<MarcaResponseDto>.Fail($"Marca no encontrada para id {request.MarcaId}.");
        }

        var nombre = request.NomMarca.Trim();
        if (await _uow.Marcas.ExisteConNombre(nombre, request.MarcaId))
        {
            return ApiResponse<MarcaResponseDto>.Fail("La marca ya existe.", ErrorType.Conflict);
        }

        entity.NomMarca = nombre;
        await _uow.Marcas.Update(entity);
        await _uow.Complete();

        return ApiResponse<MarcaResponseDto>.Ok(new MarcaResponseDto
        {
            Id = entity.Id,
            NomMarca = entity.NomMarca
        });
    }
}
