using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCodigoEntity = MixAndMatch.Domain.Entities.DescuentoCodigo;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class CreateDescuentoCodigoCommand : IRequest<ApiResponseDto<DescuentoCodigoResponseDto>>
{
    public required string Codigo { get; set; }
    public string? Descripcion { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required int UsoMaximo { get; set; }
    public required bool Activo { get; set; }
}

public class CreateDescuentoCodigoCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoCodigoCommand, ApiResponseDto<DescuentoCodigoResponseDto>>
{
    public async Task<ApiResponseDto<DescuentoCodigoResponseDto>> Handle(CreateDescuentoCodigoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo))
        {
            return ApiResponseDto<DescuentoCodigoResponseDto>.Fail("El código no puede estar vacío.");
        }

        if (request.Porcentaje < 0 || request.Porcentaje > 100)
        {
            return ApiResponseDto<DescuentoCodigoResponseDto>.Fail("El porcentaje debe estar entre 0 y 100.");
        }

        if (request.UsoMaximo <= 0)
        {
            return ApiResponseDto<DescuentoCodigoResponseDto>.Fail("El uso máximo debe ser mayor a 0.");
        }

        if (request.FechaFin.HasValue && request.FechaFin.Value < request.FechaInicio)
        {
            return ApiResponseDto<DescuentoCodigoResponseDto>.Fail("La fecha fin no puede ser menor a la fecha inicio.");
        }

        var entity = new DescuentoCodigoEntity
        {
            Codigo = request.Codigo,
            Descripcion = request.Descripcion,
            Porcentaje = request.Porcentaje,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            UsoMaximo = request.UsoMaximo,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow
        };

        var repo = _uow.Repository<DescuentoCodigoEntity>();
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponseDto<DescuentoCodigoResponseDto>.Ok(new DescuentoCodigoResponseDto
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
