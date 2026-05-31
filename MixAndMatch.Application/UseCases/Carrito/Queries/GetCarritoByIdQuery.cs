using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;

namespace MixAndMatch.Application.UseCases.Carrito.Queries;

public class GetCarritoByIdQuery : IRequest<ApiResponseDto<CarritoResponseDto>>
{
    public required long CarritoId { get; set; }
}

public class GetCarritoByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetCarritoByIdQuery, ApiResponseDto<CarritoResponseDto>>
{
    public async Task<ApiResponseDto<CarritoResponseDto>> Handle(GetCarritoByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<CarritoEntity>().GetById(request.CarritoId);
        if (entity is null)
        {
            return ApiResponseDto<CarritoResponseDto>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        return ApiResponseDto<CarritoResponseDto>.Ok(new CarritoResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
