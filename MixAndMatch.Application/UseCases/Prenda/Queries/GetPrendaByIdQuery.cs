using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetPrendaByIdQuery : IRequest<ApiResponse<PrendaResponseDto>>
{
    public required long PrendaId { get; set; }
}

public class GetPrendaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetPrendaByIdQuery, ApiResponse<PrendaResponseDto>>
{
    public async Task<ApiResponse<PrendaResponseDto>> Handle(GetPrendaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Prendas.GetById(request.PrendaId);
        if (entity is null)
        {
            return ApiResponse<PrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");
        }

        return ApiResponse<PrendaResponseDto>.Ok(new PrendaResponseDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            MarcaId = entity.MarcaId,
            CategoriaId = entity.CategoriaId,
            ProveedorId = entity.ProveedorId,
            GeneroId = entity.GeneroId,
            Precio = entity.Precio,
            Activo = entity.Activo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
