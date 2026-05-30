using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class UpdateCategoriaCommand : IRequest<ApiResponseDto<CategoriaResponseDto>>
{
    public required long CategoriaId { get; set; }
    public required string NomCategoria { get; set; }
}

public class UpdateCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateCategoriaCommand, ApiResponseDto<CategoriaResponseDto>>
{
    public async Task<ApiResponseDto<CategoriaResponseDto>> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CategoriaEntity>();
        var entity = await repo.GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponseDto<CategoriaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.");
        }

        var nombre = (request.NomCategoria ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponseDto<CategoriaResponseDto>.Fail("El nombre de la categoría es obligatorio.");
        }

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.CategoriaId && x.NomCategoria == nombre))
        {
            return ApiResponseDto<CategoriaResponseDto>.Fail("La categoría ya existe.");
        }

        entity.NomCategoria = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponseDto<CategoriaResponseDto>.Ok(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
