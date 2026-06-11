using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Carrito;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;

namespace MixAndMatch.Application.UseCases.Carrito.Queries;

public class GetCarritoByIdQuery : IRequest<ApiResponse<CarritoResponseDto>>
{
    public required long CarritoId { get; set; }
}

public class GetCarritoByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetCarritoByIdQuery, ApiResponse<CarritoResponseDto>>
{
    public async Task<ApiResponse<CarritoResponseDto>> Handle(GetCarritoByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<CarritoEntity>().GetById(request.CarritoId);
        if (entity is null)
        {
            return ApiResponse<CarritoResponseDto>.Fail($"Carrito no encontrado para id {request.CarritoId}.");
        }

        return ApiResponse<CarritoResponseDto>.Ok(new CarritoResponseDto
        {
            Id = entity.Id,
            UsuarioId = entity.UsuarioId,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
