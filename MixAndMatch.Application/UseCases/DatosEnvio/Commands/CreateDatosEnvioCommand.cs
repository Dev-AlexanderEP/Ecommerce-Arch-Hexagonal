using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Commands;

public class CreateDatosEnvioCommand : IRequest<ApiResponse<DatosEnvioResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde el token, nunca el body
    public long UsuarioId { get; set; }

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

public class CreateDatosEnvioCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateDatosEnvioCommand, ApiResponse<DatosEnvioResponseDto>>
{
    public async Task<ApiResponse<DatosEnvioResponseDto>> Handle(
        CreateDatosEnvioCommand request,
        CancellationToken cancellationToken)
    {
        if (await _uow.DatosEnvios.ExistePorUsuario(request.UsuarioId))
            return ApiResponse<DatosEnvioResponseDto>
                .Fail("El usuario ya tiene datos de envío registrados.", ErrorType.Conflict);

        var entity = new DatosEnvioEntity
        {
            UsuarioId = request.UsuarioId,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Dni = request.Dni,
            Departamento = request.Departamento,
            Provincia = request.Provincia,
            Distrito = request.Distrito,
            Calle = request.Calle,
            Detalle = request.Detalle,
            Telefono = request.Telefono,
            Email = request.Email,
            EsPrincipal = true,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.DatosEnvios.Add(entity);
        await _uow.Complete();

        return ApiResponse<DatosEnvioResponseDto>.Created(
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