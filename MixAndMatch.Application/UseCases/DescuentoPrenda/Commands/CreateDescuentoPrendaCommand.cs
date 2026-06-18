using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoPrendaEntity = MixAndMatch.Domain.Entities.DescuentoPrenda;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Commands;

public class CreateDescuentoPrendaCommand : IRequest<ApiResponse<DescuentoPrendaResponseDto>>
{
    public required long PrendaId { get; set; }
    public required decimal Porcentaje { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public required bool Activo { get; set; }
}

public class CreateDescuentoPrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateDescuentoPrendaCommand, ApiResponse<DescuentoPrendaResponseDto>>
{
    public async Task<ApiResponse<DescuentoPrendaResponseDto>> Handle(CreateDescuentoPrendaCommand request, CancellationToken cancellationToken)
    {
        var prenda = await _uow.Prendas.GetById(request.PrendaId);
        if (prenda is null)
        {
            return ApiResponse<DescuentoPrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.", ErrorType.Validation);
        }

        var entity = new DescuentoPrendaEntity
        {
            PrendaId = request.PrendaId,
            Porcentaje = request.Porcentaje,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<DescuentoPrendaEntity>().Add(entity);
        await _uow.Complete();

        return ApiResponse<DescuentoPrendaResponseDto>.Created(new DescuentoPrendaResponseDto
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
