using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Commands;

public class UpdateDescuentoPrendaCommand : IRequest<ApiResponse<DescuentoPrendaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long DescuentoPrendaId { get; set; }
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
        var entity = await _uow.Repository<DescuentoPrendaEntity>().GetById(request.DescuentoPrendaId);
        if (entity is null)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail($"Descuento de prenda no encontrado para id {request.DescuentoPrendaId}.");
        }

        var prenda = await _uow.Prendas.GetById(request.PrendaId);
        if (prenda is null)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.", ErrorType.Validation);
        }

        entity.PrendaId = request.PrendaId;
        entity.Porcentaje = request.Porcentaje;
        entity.FechaInicio = request.FechaInicio;
        entity.FechaFin = request.FechaFin;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Repository<DescuentoPrendaEntity>().Update(entity);
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
