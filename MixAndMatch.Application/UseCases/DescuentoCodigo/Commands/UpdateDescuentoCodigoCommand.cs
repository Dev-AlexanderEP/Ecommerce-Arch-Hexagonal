using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCodigoEntity = MixAndMatch.Domain.Entities.DescuentoCodigo;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class UpdateDescuentoCodigoCommand : IRequest<ApiResponse<DescuentoCodigoResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long DescuentoCodigoId { get; set; }
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
        var entity = await _uow.DescuentoCodigos.GetById(request.DescuentoCodigoId);
        if (entity is null)
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail($"Descuento de código no encontrado para id {request.DescuentoCodigoId}.");
        }

        var codigo = request.Codigo.Trim();
        if (await _uow.DescuentoCodigos.ExisteConCodigo(codigo, request.DescuentoCodigoId))
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail("El código de descuento ya existe.", ErrorType.Conflict);
        }

        entity.Codigo = codigo;
        entity.Descripcion = request.Descripcion;
        entity.Porcentaje = request.Porcentaje;
        entity.FechaInicio = request.FechaInicio;
        entity.FechaFin = request.FechaFin;
        entity.UsoMaximo = request.UsoMaximo;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.DescuentoCodigos.Update(entity);
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
