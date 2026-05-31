using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Commands;

public class CreateDescuentoPrendaCommand : IRequest<ApiResponseDto<DescuentoPrendaResponseDto>>
{
    public required long PrendaId { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required bool Activo { get; set; }
}

public class CreateDescuentoPrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoPrendaCommand, ApiResponseDto<DescuentoPrendaResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoPrendaResponseDto>> Handle(CreateDescuentoPrendaCommand request, CancellationToken cancellationToken)
    {
        if (request.Porcentaje < 0 || request.Porcentaje > 100)
        {
            return ApiResponseDto<DescuentoPrendaResponseDto>.Fail("El porcentaje debe estar entre 0 y 100.");
        }

        if (request.FechaFin.HasValue && request.FechaFin.Value < request.FechaInicio)
        {
            return ApiResponseDto<DescuentoPrendaResponseDto>.Fail("La fecha fin no puede ser menor a la fecha inicio.");
        }

        var prenda = await _uow.Repository<PrendaEntity>().GetById(request.PrendaId);
        if (prenda is null)
        {
            return ApiResponseDto<DescuentoPrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");
        }

        var entity = new DescuentoPrendaEntity
        {
            PrendaId = request.PrendaId,
            Porcentaje = request.Porcentaje,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow
        };

        var repo = _uow.Repository<DescuentoPrendaEntity>();
        await repo.Add(entity);
        await _uow.Complete();

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
