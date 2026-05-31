using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CarritoEntity = MixAndMatch.Domain.Entities.Carrito;

namespace MixAndMatch.Application.UseCases.Carrito.Queries;

public class GetAllCarritosQuery : IRequest<ApiResponseDto<IEnumerable<CarritoResponseDto>>>
{
}

public class GetAllCarritosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllCarritosQuery, ApiResponseDto<IEnumerable<CarritoResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<CarritoResponseDto>>> Handle(GetAllCarritosQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<CarritoEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<CarritoResponseDto>>.Fail("No se encontraron carritos.");
        }

        return ApiResponseDto<IEnumerable<CarritoResponseDto>>.Ok(items.Select(x => new CarritoResponseDto
        {
            Id = x.Id,
            UsuarioId = x.UsuarioId,
            FechaCreacion = x.FechaCreacion,
            Estado = x.Estado,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
