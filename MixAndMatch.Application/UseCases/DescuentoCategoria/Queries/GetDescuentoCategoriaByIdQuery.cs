using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Queries;

public class GetDescuentoCategoriaByIdQuery : IRequest<ApiResponseDto<DescuentoCategoriaResponseDto>>
{
    public required long DescuentoCategoriaId { get; set; }
}

public class GetDescuentoCategoriaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentoCategoriaByIdQuery, ApiResponseDto<DescuentoCategoriaResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoCategoriaResponseDto>> Handle(GetDescuentoCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DescuentoCategoriaEntity>().GetById(request.DescuentoCategoriaId);
        if (entity is null)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail($"Descuento de categoría no encontrado para id {request.DescuentoCategoriaId}.");
        }

        return ApiResponseDto<DescuentoCategoriaResponseDto>.Ok(new DescuentoCategoriaResponseDto
        {
            Id = entity.Id,
            CategoriaId = entity.CategoriaId,
            Porcentaje = entity.Porcentaje,
            FechaInicio = entity.FechaInicio,
            FechaFin = entity.FechaFin,
            Activo = entity.Activo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
