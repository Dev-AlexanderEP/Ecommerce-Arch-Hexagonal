using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoUsuarioEntity = MixAndMatch.Domain.Entities.DescuentoUsuario;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Queries;

public class GetAllDescuentoUsuariosQuery : IRequest<ApiResponseDto<IEnumerable<DescuentoUsuarioResponseDto>>>
{
}

public class GetAllDescuentoUsuariosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoUsuariosQuery, ApiResponseDto<IEnumerable<DescuentoUsuarioResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<DescuentoUsuarioResponseDto>>> Handle(GetAllDescuentoUsuariosQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<DescuentoUsuarioEntity>().GetAll();
        if (!items.Any())
        {
            return ApiResponseDto<IEnumerable<DescuentoUsuarioResponseDto>>.Fail("No se encontraron registros de uso de descuentos.");
        }

        return ApiResponseDto<IEnumerable<DescuentoUsuarioResponseDto>>.Ok(items.Select(x => new DescuentoUsuarioResponseDto
        {
            Id = x.Id,
            DescuentoCodigoId = x.DescuentoCodigoId,
            UsuarioId = x.UsuarioId,
            FechaUso = x.FechaUso,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }));
    }
}
