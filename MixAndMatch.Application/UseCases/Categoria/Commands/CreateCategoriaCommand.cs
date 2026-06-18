using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class CreateCategoriaCommand : IRequest<ApiResponse<CategoriaResponseDto>>
{
    public required string NomCategoria { get; set; }
}

public class CreateCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateCategoriaCommand, ApiResponse<CategoriaResponseDto>>
{
    public async Task<ApiResponse<CategoriaResponseDto>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var nombre = request.NomCategoria.Trim();

        if (await _uow.Categorias.ExisteConNombre(nombre))
        {
            return ApiResponse<CategoriaResponseDto>.Fail("La categoría ya existe.", ErrorType.Conflict);
        }

        var entity = new CategoriaEntity { NomCategoria = nombre };
        await _uow.Categorias.Add(entity);
        await _uow.Complete();

        return ApiResponse<CategoriaResponseDto>.Created(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
