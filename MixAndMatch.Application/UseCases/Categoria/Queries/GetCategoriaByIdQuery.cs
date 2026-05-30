using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Queries;

public class GetCategoriaByIdQuery : IRequest<ApiResponseDto<CategoriaResponseDto>>
{
    public required long CategoriaId { get; set; }
}

public class GetCategoriaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetCategoriaByIdQuery, ApiResponseDto<CategoriaResponseDto>>
{
    public async Task<ApiResponseDto<CategoriaResponseDto>> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<CategoriaEntity>().GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponseDto<CategoriaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.");
        }

        return ApiResponseDto<CategoriaResponseDto>.Ok(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
