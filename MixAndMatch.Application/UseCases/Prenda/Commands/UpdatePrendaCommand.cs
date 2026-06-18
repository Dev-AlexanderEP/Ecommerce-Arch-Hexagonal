using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Prenda.Commands;

public class UpdatePrendaCommand : IRequest<ApiResponse<PrendaResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long PrendaId { get; set; }
    public required string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public required long MarcaId { get; set; }
    public required long CategoriaId { get; set; }
    public required long ProveedorId { get; set; }
    public required long GeneroId { get; set; }
    public required decimal Precio { get; set; }
    public required bool Activo { get; set; }
}

public class UpdatePrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdatePrendaCommand, ApiResponse<PrendaResponseDto>>
{
    public async Task<ApiResponse<PrendaResponseDto>> Handle(UpdatePrendaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Prendas.GetById(request.PrendaId);
        if (entity is null)
        {
            return ApiResponse<PrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");
        }

        var nombre = request.Nombre.Trim();

        if (await _uow.Marcas.GetById(request.MarcaId) is null)
        {
            return ApiResponse<PrendaResponseDto>.Fail($"Marca no encontrada para id {request.MarcaId}.", ErrorType.Validation);
        }

        if (await _uow.Categorias.GetById(request.CategoriaId) is null)
        {
            return ApiResponse<PrendaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.", ErrorType.Validation);
        }

        if (await _uow.Proveedores.GetById(request.ProveedorId) is null)
        {
            return ApiResponse<PrendaResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.", ErrorType.Validation);
        }

        if (await _uow.Generos.GetById(request.GeneroId) is null)
        {
            return ApiResponse<PrendaResponseDto>.Fail($"Género no encontrado para id {request.GeneroId}.", ErrorType.Validation);
        }

        entity.Nombre = nombre;
        entity.Descripcion = request.Descripcion;
        entity.MarcaId = request.MarcaId;
        entity.CategoriaId = request.CategoriaId;
        entity.ProveedorId = request.ProveedorId;
        entity.GeneroId = request.GeneroId;
        entity.Precio = request.Precio;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.Prendas.Update(entity);
        await _uow.Complete();

        return ApiResponse<PrendaResponseDto>.Ok(new PrendaResponseDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            MarcaId = entity.MarcaId,
            CategoriaId = entity.CategoriaId,
            ProveedorId = entity.ProveedorId,
            GeneroId = entity.GeneroId,
            Precio = entity.Precio,
            Activo = entity.Activo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
