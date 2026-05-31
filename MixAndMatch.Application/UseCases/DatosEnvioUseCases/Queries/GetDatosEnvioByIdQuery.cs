using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Queries;

public class GetDatosEnvioByIdQuery : IRequest<ApiResponseDto<DatosEnvioResponseDto>>
{
    public required long DatosEnvioId { get; set; }
}

public class GetDatosEnvioByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetDatosEnvioByIdQuery, ApiResponseDto<DatosEnvioResponseDto>>
{
    public async Task<ApiResponseDto<DatosEnvioResponseDto>> Handle(
        GetDatosEnvioByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DatosEnvioEntity>()
            .GetById(request.DatosEnvioId);

        if (entity is null)
            return ApiResponseDto<DatosEnvioResponseDto>
                .Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.");

        return ApiResponseDto<DatosEnvioResponseDto>.Ok(
            new DatosEnvioResponseDto
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
            });
    }
}