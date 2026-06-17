using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCategoriaEntity = MixAndMatch.Domain.Entities.DescuentoCategoria;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;

public class CreateDescuentoCategoriaCommand : IRequest<ApiResponse<DescuentoCategoriaResponseDto>>
{
    public required long CategoriaId { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required bool Activo { get; set; }
}

public class CreateDescuentoCategoriaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoCategoriaCommand, ApiResponse<DescuentoCategoriaResponseDto>>
{
    public async Task<ApiResponse<DescuentoCategoriaResponseDto>> Handle(CreateDescuentoCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _uow.Categorias.GetById(request.CategoriaId);
        if (categoria is null)
        {
            return ApiResponse<DescuentoCategoriaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.", ErrorType.Validation);
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

        await _uow.Repository<DescuentoCategoriaEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponse<DescuentoCategoriaResponseDto>.Created(new DescuentoCategoriaResponseDto
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
