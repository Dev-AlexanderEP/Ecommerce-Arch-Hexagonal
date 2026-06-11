using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Commands;

public class UpdateDescuentoPrendaCommand : IRequest<ApiResponse<DescuentoPrendaResponseDto>>
{
    public required long DescuentoPrendaId { get; set; }
    public required long PrendaId { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required bool Activo { get; set; }
}

public class UpdateDescuentoPrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateDescuentoPrendaCommand, ApiResponse<DescuentoPrendaResponseDto>>
{
    public async Task<ApiResponse<DescuentoPrendaResponseDto>> Handle(UpdateDescuentoPrendaCommand request, CancellationToken cancellationToken)
    {
        if (request.Porcentaje < 0 || request.Porcentaje > 100)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail("El porcentaje debe estar entre 0 y 100.");
        }

        if (request.FechaFin.HasValue && request.FechaFin.Value < request.FechaInicio)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail("La fecha fin no puede ser menor a la fecha inicio.");
        }

        var repo = _uow.Repository<DescuentoPrendaEntity>();
        var entity = await repo.GetById(request.DescuentoPrendaId);
        if (entity is null)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail($"Descuento de prenda no encontrado para id {request.DescuentoPrendaId}.");
        }

        var prenda = await _uow.Repository<PrendaEntity>().GetById(request.PrendaId);
        if (prenda is null)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");
        }

        entity.PrendaId = request.PrendaId;
        entity.Porcentaje = request.Porcentaje;
        entity.FechaInicio = request.FechaInicio;
        entity.FechaFin = request.FechaFin;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

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
