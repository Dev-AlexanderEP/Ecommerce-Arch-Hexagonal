using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;

public class UpdateDescuentoCategoriaCommand : IRequest<ApiResponse<DescuentoCategoriaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long DescuentoCategoriaId { get; set; }
    public required long CategoriaId { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required bool Activo { get; set; }
}

public class UpdateDescuentoCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateDescuentoCategoriaCommand, ApiResponse<DescuentoCategoriaResponseDto>>
{
    public async Task<ApiResponse<DescuentoCategoriaResponseDto>> Handle(UpdateDescuentoCategoriaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DescuentoCategoriaEntity>().GetById(request.DescuentoCategoriaId);
        if (entity is null)
        {
            return ApiResponse<DescuentoCategoriaResponseDto>.Fail($"Descuento de categoría no encontrado para id {request.DescuentoCategoriaId}.");
        }

        var categoria = await _uow.Categorias.GetById(request.CategoriaId);
        if (categoria is null)
        {
            return ApiResponse<DescuentoCategoriaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.", ErrorType.Validation);
        }

        entity.CategoriaId = request.CategoriaId;
        entity.Porcentaje = request.Porcentaje;
        entity.FechaInicio = request.FechaInicio;
        entity.FechaFin = request.FechaFin;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Repository<DescuentoCategoriaEntity>().Update(entity);
        await _uow.Complete();

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
