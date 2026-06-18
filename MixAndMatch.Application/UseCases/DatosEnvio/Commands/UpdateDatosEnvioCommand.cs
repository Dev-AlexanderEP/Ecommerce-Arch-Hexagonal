using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Commands;

public class UpdateDatosEnvioCommand : IRequest<ApiResponse<DatosEnvioResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long DatosEnvioId { get; set; }

    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long SolicitanteId { get; set; }

    public required string Nombres { get; set; }
    public required string Apellidos { get; set; }
    public required string Dni { get; set; }
    public required string Departamento { get; set; }
    public required string Provincia { get; set; }
    public required string Distrito { get; set; }
    public string? Calle { get; set; }
    public required string Detalle { get; set; }
    public required string Telefono { get; set; }
    public required string Email { get; set; }
}

public class UpdateDatosEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdateDatosEnvioCommand, ApiResponse<DatosEnvioResponseDto>>
{
    public async Task<ApiResponse<DatosEnvioResponseDto>> Handle(
        UpdateDatosEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.DatosEnvios.GetById(request.DatosEnvioId);

        if (entity is null)
            return ApiResponse<DatosEnvioResponseDto>
                .Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.");

        if (entity.UsuarioId != request.SolicitanteId)
            return ApiResponse<DatosEnvioResponseDto>
                .Fail("No tienes acceso a estos datos de envío.", ErrorType.Forbidden);

        entity.Nombres = request.Nombres;
        entity.Apellidos = request.Apellidos;
        entity.Dni = request.Dni;
        entity.Departamento = request.Departamento;
        entity.Provincia = request.Provincia;
        entity.Distrito = request.Distrito;
        entity.Calle = request.Calle;
        entity.Detalle = request.Detalle;
        entity.Telefono = request.Telefono;
        entity.Email = request.Email;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.DatosEnvios.Update(entity);
        await _uow.Complete();

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