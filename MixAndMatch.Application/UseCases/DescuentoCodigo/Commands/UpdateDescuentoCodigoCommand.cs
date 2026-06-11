using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCodigoEntity = MixAndMatch.Domain.Entities.DescuentoCodigo;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class UpdateDescuentoCodigoCommand : IRequest<ApiResponse<DescuentoCodigoResponseDto>>
{
    public required long DescuentoCodigoId { get; set; }
    public required string Codigo { get; set; }
    public string? Descripcion { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required int UsoMaximo { get; set; }
    public required bool Activo { get; set; }
}

public class UpdateDescuentoCodigoCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateDescuentoCodigoCommand, ApiResponse<DescuentoCodigoResponseDto>>
{
    public async Task<ApiResponse<DescuentoCodigoResponseDto>> Handle(UpdateDescuentoCodigoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo))
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail("El código no puede estar vacío.");
        }

        if (request.Porcentaje < 0 || request.Porcentaje > 100)
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail("El porcentaje debe estar entre 0 y 100.");
        }

        if (request.UsoMaximo <= 0)
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail("El uso máximo debe ser mayor a 0.");
        }

        if (request.FechaFin.HasValue && request.FechaFin.Value < request.FechaInicio)
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail("La fecha fin no puede ser menor a la fecha inicio.");
        }

        var repo = _uow.Repository<DescuentoCodigoEntity>();
        var entity = await repo.GetById(request.DescuentoCodigoId);
        if (entity is null)
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail($"Descuento de código no encontrado para id {request.DescuentoCodigoId}.");
        }

        entity.Codigo = request.Codigo;
        entity.Descripcion = request.Descripcion;
        entity.Porcentaje = request.Porcentaje;
        entity.FechaInicio = request.FechaInicio;
        entity.FechaFin = request.FechaFin;
        entity.UsoMaximo = request.UsoMaximo;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<DescuentoCodigoResponseDto>.Ok(new DescuentoCodigoResponseDto
        {
            Id = entity.Id,
            Codigo = entity.Codigo,
            Descripcion = entity.Descripcion,
            Porcentaje = entity.Porcentaje,
            FechaInicio = entity.FechaInicio,
            FechaFin = entity.FechaFin,
            UsoMaximo = entity.UsoMaximo,
            Activo = entity.Activo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
