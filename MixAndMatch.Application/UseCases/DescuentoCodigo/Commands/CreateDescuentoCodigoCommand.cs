using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoCodigoEntity = MixAndMatch.Domain.Entities.DescuentoCodigo;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

public class CreateDescuentoCodigoCommand : IRequest<ApiResponse<DescuentoCodigoResponseDto>>
{
    public required string Codigo { get; set; }
    public string? Descripcion { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required int UsoMaximo { get; set; }
    public required bool Activo { get; set; }
}

public class CreateDescuentoCodigoCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoCodigoCommand, ApiResponse<DescuentoCodigoResponseDto>>
{
    public async Task<ApiResponse<DescuentoCodigoResponseDto>> Handle(CreateDescuentoCodigoCommand request, CancellationToken cancellationToken)
    {
        var codigo = request.Codigo.Trim();

        if (await _uow.DescuentoCodigos.ExisteConCodigo(codigo))
        {
            return ApiResponse<DescuentoCodigoResponseDto>.Fail("El código de descuento ya existe.", ErrorType.Conflict);
        }

        var entity = new DescuentoCodigoEntity
        {
            Codigo = codigo,
            Descripcion = request.Descripcion,
            Porcentaje = request.Porcentaje,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            UsoMaximo = request.UsoMaximo,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.DescuentoCodigos.Add(entity);
        await _uow.Complete();

        return ApiResponse<DescuentoCodigoResponseDto>.Created(new DescuentoCodigoResponseDto
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
