using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.DTOs.Descuentos;
using MixAndMatch.Domain.Ports.IRepositories;
using DescuentoUsuarioEntity = MixAndMatch.Domain.Entities.DescuentoUsuario;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Queries;

public class GetAllDescuentoUsuariosQuery : IRequest<ApiPaginationResponse<DescuentoUsuarioResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllDescuentoUsuariosQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetAllDescuentoUsuariosQuery, ApiPaginationResponse<DescuentoUsuarioResponseDto>>
{
    public async Task<ApiPaginationResponse<DescuentoUsuarioResponseDto>> Handle(GetAllDescuentoUsuariosQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<DescuentoUsuarioEntity>().GetPaged(request.Page, request.PageSize);
        if (!items.Any())
        {
            return ApiPaginationResponse<DescuentoUsuarioResponseDto>.Fail("No se encontraron registros de uso de descuentos.");
        }

        return ApiPaginationResponse<DescuentoUsuarioResponseDto>.Ok(items.Select(x => new DescuentoUsuarioResponseDto
        {
            Id = x.Id,
            DescuentoCodigoId = x.DescuentoCodigoId,
            UsuarioId = x.UsuarioId,
            FechaUso = x.FechaUso,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }), total, request.Page, request.PageSize);
    }
}
