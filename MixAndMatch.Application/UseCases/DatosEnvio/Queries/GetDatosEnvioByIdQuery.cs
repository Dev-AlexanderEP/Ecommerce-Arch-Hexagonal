using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Queries;

public class GetDatosEnvioByIdQuery : IRequest<ApiResponse<DatosEnvioResponseDto>>
{
    public required long DatosEnvioId { get; set; }
    public required long SolicitanteId { get; set; }
}

public class GetDatosEnvioByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetDatosEnvioByIdQuery, ApiResponse<DatosEnvioResponseDto>>
{
    public async Task<ApiResponse<DatosEnvioResponseDto>> Handle(
        GetDatosEnvioByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.DatosEnvios.GetById(request.DatosEnvioId);

        if (entity is null)
            return ApiResponse<DatosEnvioResponseDto>
                .Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.");

        if (entity.UsuarioId != request.SolicitanteId)
            return ApiResponse<DatosEnvioResponseDto>
                .Fail("No tienes acceso a estos datos de envío.", ErrorType.Forbidden);

        return ApiResponse<DatosEnvioResponseDto>.Ok(
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