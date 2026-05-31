using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;

public class UpdateDescuentoCategoriaCommand : IRequest<ApiResponseDto<DescuentoCategoriaResponseDto>>
{
    public required long DescuentoCategoriaId { get; set; }
    public required long CategoriaId { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required bool Activo { get; set; }
}

public class UpdateDescuentoCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateDescuentoCategoriaCommand, ApiResponseDto<DescuentoCategoriaResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoCategoriaResponseDto>> Handle(UpdateDescuentoCategoriaCommand request, CancellationToken cancellationToken)
    {
        if (request.Porcentaje < 0 || request.Porcentaje > 100)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail("El porcentaje debe estar entre 0 y 100.");
        }

        if (request.FechaFin.HasValue && request.FechaFin.Value < request.FechaInicio)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail("La fecha fin no puede ser menor a la fecha inicio.");
        }

        var repo = _uow.Repository<DescuentoCategoriaEntity>();
        var entity = await repo.GetById(request.DescuentoCategoriaId);
        if (entity is null)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail($"Descuento de categoría no encontrado para id {request.DescuentoCategoriaId}.");
        }

        var categoria = await _uow.Repository<CategoriaEntity>().GetById(request.CategoriaId);
        if (categoria is null)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail($"categoría no encontrada para id {request.CategoriaId}.");
        }

        entity.CategoriaId = request.CategoriaId;
        entity.Porcentaje = request.Porcentaje;
        entity.FechaInicio = request.FechaInicio;
        entity.FechaFin = request.FechaFin;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

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
