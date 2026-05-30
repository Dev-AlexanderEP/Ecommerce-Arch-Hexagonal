using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Prenda.Queries;

public class GetPrendaByIdQuery : IRequest<ApiResponseDto<PrendaResponseDto>>
{
    public required long PrendaId { get; set; }
}

public class GetPrendaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetPrendaByIdQuery, ApiResponseDto<PrendaResponseDto>>
{
    public async Task<ApiResponseDto<PrendaResponseDto>> Handle(GetPrendaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<PrendaEntity>().GetById(request.PrendaId);
        if (entity is null)
        {
            return ApiResponseDto<PrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");
        }

        return ApiResponseDto<PrendaResponseDto>.Ok(new PrendaResponseDto
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
