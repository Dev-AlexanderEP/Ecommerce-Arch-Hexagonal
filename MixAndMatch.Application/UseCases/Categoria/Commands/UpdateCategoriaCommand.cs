using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Commands;

public class UpdateCategoriaCommand : IRequest<ApiResponse<CategoriaResponseDto>>
{
    public required long CategoriaId { get; set; }
    public required string NomCategoria { get; set; }
}

public class UpdateCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateCategoriaCommand, ApiResponse<CategoriaResponseDto>>
{
    public async Task<ApiResponse<CategoriaResponseDto>> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<CategoriaEntity>();
        var entity = await repo.GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponse<CategoriaResponseDto>.Fail($"CategorÃ­a no encontrada para id {request.CategoriaId}.");
        }

        var nombre = (request.NomCategoria ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<CategoriaResponseDto>.Fail("El nombre de la categorÃ­a es obligatorio.");
        }

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.CategoriaId && x.NomCategoria == nombre))
        {
            return ApiResponse<CategoriaResponseDto>.Fail("La categorÃ­a ya existe.");
        }

        entity.NomCategoria = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<CategoriaResponseDto>.Ok(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
