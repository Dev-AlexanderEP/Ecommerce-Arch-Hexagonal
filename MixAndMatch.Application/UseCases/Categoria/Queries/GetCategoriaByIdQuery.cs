using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;

namespace MixAndMatch.Application.UseCases.Categoria.Queries;

public class GetCategoriaByIdQuery : IRequest<ApiResponse<CategoriaResponseDto>>
{
    public required long CategoriaId { get; set; }
}

public class GetCategoriaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetCategoriaByIdQuery, ApiResponse<CategoriaResponseDto>>
{
    public async Task<ApiResponse<CategoriaResponseDto>> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<CategoriaEntity>().GetById(request.CategoriaId);
        if (entity is null)
        {
            return ApiResponse<CategoriaResponseDto>.Fail($"CategorÃ­a no encontrada para id {request.CategoriaId}.");
        }

        return ApiResponse<CategoriaResponseDto>.Ok(new CategoriaResponseDto
        {
            Id = entity.Id,
            NomCategoria = entity.NomCategoria
        });
    }
}
