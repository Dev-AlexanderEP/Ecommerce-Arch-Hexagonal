using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Queries;

public class GetDescuentoPrendaByIdQuery : IRequest<ApiResponse<DescuentoPrendaResponseDto>>
{
    public required long DescuentoPrendaId { get; set; }
}

public class GetDescuentoPrendaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentoPrendaByIdQuery, ApiResponse<DescuentoPrendaResponseDto>>
{
    public async Task<ApiResponse<DescuentoPrendaResponseDto>> Handle(GetDescuentoPrendaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DescuentoPrendaEntity>().GetById(request.DescuentoPrendaId);
        if (entity is null)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail($"Descuento de prenda no encontrado para id {request.DescuentoPrendaId}.");
        }

        return ApiResponse<DescuentoPrendaResponseDto>.Ok(new DescuentoPrendaResponseDto
        {
            Id = entity.Id,
            PrendaId = entity.PrendaId,
            Porcentaje = entity.Porcentaje,
            FechaInicio = entity.FechaInicio,
            FechaFin = entity.FechaFin,
            Activo = entity.Activo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
