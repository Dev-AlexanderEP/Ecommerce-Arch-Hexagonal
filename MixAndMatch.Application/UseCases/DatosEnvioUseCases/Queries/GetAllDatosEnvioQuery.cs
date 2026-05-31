using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Queries;

public class GetAllDatosEnvioQuery : IRequest<ApiResponseDto<IEnumerable<DatosEnvioResponseDto>>>
{
}

public class GetAllDatosEnvioQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetAllDatosEnvioQuery, ApiResponseDto<IEnumerable<DatosEnvioResponseDto>>>
{
    public async Task<ApiResponseDto<IEnumerable<DatosEnvioResponseDto>>> Handle(
        GetAllDatosEnvioQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _uow.Repository<DatosEnvioEntity>().GetAll();

        if (!items.Any())
            return ApiResponseDto<IEnumerable<DatosEnvioResponseDto>>
                .Fail("No se encontraron datos de envío.");

        return ApiResponseDto<IEnumerable<DatosEnvioResponseDto>>.Ok(
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
            }));
    }
}