using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using DatosEnvioEntity = MixAndMatch.Domain.Entities.DatosEnvio;
using UsuarioEntity = MixAndMatch.Domain.Entities.Usuario;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Commands;

public class UpdateDatosEnvioCommand : IRequest<ApiResponseDto<DatosEnvioResponseDto>>
{
    public required long DatosEnvioId { get; set; }
    public required long UsuarioId { get; set; }

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
    : IRequestHandler<UpdateDatosEnvioCommand, ApiResponseDto<DatosEnvioResponseDto>>
{
    public async Task<ApiResponseDto<DatosEnvioResponseDto>> Handle(
        UpdateDatosEnvioCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<DatosEnvioEntity>()
            .GetById(request.DatosEnvioId);

        if (entity is null)
            return ApiResponseDto<DatosEnvioResponseDto>
                .Fail($"Datos de envío no encontrados para id {request.DatosEnvioId}.");

        var usuario = await _uow.Repository<UsuarioEntity>()
            .GetById(request.UsuarioId);

        if (usuario is null)
            return ApiResponseDto<DatosEnvioResponseDto>
                .Fail($"Usuario no encontrado para id {request.UsuarioId}.");

        entity.UsuarioId = request.UsuarioId;
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

        await _uow.Repository<DatosEnvioEntity>().Update(entity);
        await _uow.Complete();

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