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
        var nombre = (request.NomCategoria ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<CategoriaResponseDto>.Fail("El nombre de la categorÃ­a es obligatorio.");
        }

        var repo = _uow.Repository<CategoriaEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomCategoria == nombre))
        {
            return ApiResponse<CategoriaResponseDto>.Fail("La categorÃ­a ya existe.");
        }

        var entity = new CategoriaEntity { NomCategoria = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponse<CategoriaResponseDto>.Ok(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
