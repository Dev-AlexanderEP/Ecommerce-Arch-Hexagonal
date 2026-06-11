using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Queries;

public class GetDescuentoCategoriaByIdQuery : IRequest<ApiResponse<DescuentoCategoriaResponseDto>>
{
    public required long DescuentoCategoriaId { get; set; }
}

public class GetDescuentoCategoriaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentoCategoriaByIdQuery, ApiResponse<DescuentoCategoriaResponseDto>>
{
    public async Task<ApiResponse<DescuentoCategoriaResponseDto>> Handle(GetDescuentoCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DescuentoCategoriaEntity>().GetById(request.DescuentoCategoriaId);
        if (entity is null)
        {
            return ApiResponse<DescuentoCategoriaResponseDto>.Fail($"Descuento de categorÃ­a no encontrado para id {request.DescuentoCategoriaId}.");
        }

        return ApiResponse<DescuentoCategoriaResponseDto>.Ok(new DescuentoCategoriaResponseDto
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
