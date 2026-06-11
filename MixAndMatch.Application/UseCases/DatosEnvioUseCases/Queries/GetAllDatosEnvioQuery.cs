using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Queries;

public class GetAllDatosEnvioQuery : IRequest<ApiPaginationResponse<DatosEnvioResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllDatosEnvioQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllDatosEnvioQuery, ApiPaginationResponse<DatosEnvioResponseDto>>
{
    public async Task<ApiPaginationResponse<DatosEnvioResponseDto>> Handle(
        GetAllDatosEnvioQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await _uow.Repository<DatosEnvioEntity>().GetPaged(request.Page, request.PageSize);

        if (!items.Any())
            return ApiPaginationResponse<DatosEnvioResponseDto>
                .Fail("No se encontraron datos de envío.");

        return ApiPaginationResponse<DatosEnvioResponseDto>.Ok(
            items.Select(entity => new DatosEnvioResponseDto
            {
                Id = entity.Id,
                UsuarioId = entity.UsuarioId,
                Nombres = entity.Nombres,
                Apellidos = entity.Apellidos,
                Dni = entity.Dni,
                Departamento = entity.Departamento,
                Provincia = entity.Provincia,
                Distrito = entity.Distrito,
                Calle = entity.Calle,
                Detalle = entity.Detalle,
                Telefono = entity.Telefono,
                Email = entity.Email,
                EsPrincipal = entity.EsPrincipal,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            }), total, request.Page, request.PageSize);
    }
}