using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;

public class CreateDescuentoCategoriaCommand : IRequest<ApiResponseDto<DescuentoCategoriaResponseDto>>
{
    public required long CategoriaId { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required bool Activo { get; set; }
}

public class CreateDescuentoCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoCategoriaCommand, ApiResponseDto<DescuentoCategoriaResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoCategoriaResponseDto>> Handle(CreateDescuentoCategoriaCommand request, CancellationToken cancellationToken)
    {
        if (request.Porcentaje < 0 || request.Porcentaje > 100)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail("El porcentaje debe estar entre 0 y 100.");
        }

        if (request.FechaFin.HasValue && request.FechaFin.Value < request.FechaInicio)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail("La fecha fin no puede ser menor a la fecha inicio.");
        }

        var categoria = await _uow.Repository<CategoriaEntity>().GetById(request.CategoriaId);
        if (categoria is null)
        {
            return ApiResponseDto<DescuentoCategoriaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.");
        }

        var entity = new DescuentoCategoriaEntity
        {
            CategoriaId = request.CategoriaId,
            Porcentaje = request.Porcentaje,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow
        };

        var repo = _uow.Repository<DescuentoCategoriaEntity>();
        await repo.Add(entity);
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
