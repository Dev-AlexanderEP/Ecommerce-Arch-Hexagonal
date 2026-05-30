using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class CreateCategoriaCommand : IRequest<ApiResponseDto<CategoriaResponseDto>>
{
    public required string NomCategoria { get; set; }
}

public class CreateCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateCategoriaCommand, ApiResponseDto<CategoriaResponseDto>>
{
    public async Task<ApiResponseDto<CategoriaResponseDto>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.NomCategoria ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponseDto<CategoriaResponseDto>.Fail("El nombre de la categoría es obligatorio.");
        }

        var repo = _uow.Repository<CategoriaEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomCategoria == nombre))
        {
            return ApiResponseDto<CategoriaResponseDto>.Fail("La categoría ya existe.");
        }

        var entity = new CategoriaEntity { NomCategoria = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponseDto<CategoriaResponseDto>.Ok(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
