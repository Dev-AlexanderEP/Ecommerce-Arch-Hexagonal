using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Queries;

public class GetDescuentoPrendaByIdQuery : IRequest<ApiResponseDto<DescuentoPrendaResponseDto>>
{
    public required long DescuentoPrendaId { get; set; }
}

public class GetDescuentoPrendaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetDescuentoPrendaByIdQuery, ApiResponseDto<DescuentoPrendaResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoPrendaResponseDto>> Handle(GetDescuentoPrendaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DescuentoPrendaEntity>().GetById(request.DescuentoPrendaId);
        if (entity is null)
        {
            return ApiResponseDto<DescuentoPrendaResponseDto>.Fail($"Descuento de prenda no encontrado para id {request.DescuentoPrendaId}.");
        }

        return ApiResponseDto<DescuentoPrendaResponseDto>.Ok(new DescuentoPrendaResponseDto
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
