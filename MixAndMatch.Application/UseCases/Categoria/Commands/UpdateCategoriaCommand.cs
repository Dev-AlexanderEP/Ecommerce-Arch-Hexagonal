using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class UpdateCategoriaCommand : IRequest<ApiResponse<CategoriaResponseDto>>
{
    [JsonIgnore]
    public long CategoriaId { get; set; }
    public required string NomCategoria { get; set; }
}

public class UpdateCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateCategoriaCommand, ApiResponse<CategoriaResponseDto>>
{
    public async Task<ApiResponse<CategoriaResponseDto>> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Categorias.GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponse<CategoriaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.");
        }

        var nombre = request.NomCategoria.Trim();
        if (await _uow.Categorias.ExisteConNombre(nombre, request.CategoriaId))
        {
            return ApiResponse<CategoriaResponseDto>.Fail("La categoría ya existe.", ErrorType.Conflict);
        }

        entity.NomCategoria = nombre;
        await _uow.Categorias.Update(entity);
        await _uow.Complete();

        return ApiResponse<CategoriaResponseDto>.Ok(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
