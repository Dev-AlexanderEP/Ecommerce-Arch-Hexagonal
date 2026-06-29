using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Queries;

public class GetMisDireccionesQuery : IRequest<ApiResponse<List<DatosEnvioResponseDto>>>
{
    [JsonIgnore]
    public long UsuarioId { get; set; }
}

public class GetMisDireccionesQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetMisDireccionesQuery, ApiResponse<List<DatosEnvioResponseDto>>>
{
    public async Task<ApiResponse<List<DatosEnvioResponseDto>>> Handle(
        GetMisDireccionesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _uow.DatosEnvios.GetByUsuarioId(request.UsuarioId);

        return ApiResponse<List<DatosEnvioResponseDto>>.Ok(
            items.Select(e => new DatosEnvioResponseDto
            {
                Id           = e.Id,
                UsuarioId    = e.UsuarioId,
                Nombres      = e.Nombres,
                Apellidos    = e.Apellidos,
                Dni          = e.Dni,
                Departamento = e.Departamento,
                Provincia    = e.Provincia,
                Distrito     = e.Distrito,
                Calle        = e.Calle,
                Detalle      = e.Detalle,
                Telefono     = e.Telefono,
                Email        = e.Email,
                EsPrincipal  = e.EsPrincipal,
                CreatedAt    = e.CreatedAt,
                UpdatedAt    = e.UpdatedAt
            }).ToList());
    }
}
