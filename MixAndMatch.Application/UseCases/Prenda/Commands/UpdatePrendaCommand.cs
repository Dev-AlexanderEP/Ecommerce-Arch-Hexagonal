using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Prenda.Commands;

public class UpdatePrendaCommand : IRequest<ApiResponseDto<PrendaResponseDto>>
{
    public required long PrendaId { get; set; }
    public required string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public required long MarcaId { get; set; }
    public required long CategoriaId { get; set; }
    public required long ProveedorId { get; set; }
    public required long GeneroId { get; set; }
    public required decimal Precio { get; set; }
    public required bool Activo { get; set; }
}

public class UpdatePrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdatePrendaCommand, ApiResponseDto<PrendaResponseDto>>
{
    public async Task<ApiResponseDto<PrendaResponseDto>> Handle(UpdatePrendaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<PrendaEntity>();
        var entity = await repo.GetById(request.PrendaId);
        if (entity is null)
        {
            return ApiResponseDto<PrendaResponseDto>.Fail($"Prenda no encontrada para id {request.PrendaId}.");
        }

        var nombre = (request.Nombre ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponseDto<PrendaResponseDto>.Fail("El nombre de la prenda es obligatorio.");
        }

        if (request.Precio < 0)
        {
            return ApiResponseDto<PrendaResponseDto>.Fail("El precio no puede ser negativo.");
        }

        if (await _uow.Repository<MarcaEntity>().GetById(request.MarcaId) is null) return ApiResponseDto<PrendaResponseDto>.Fail($"Marca no encontrada para id {request.MarcaId}.");
        if (await _uow.Repository<CategoriaEntity>().GetById(request.CategoriaId) is null) return ApiResponseDto<PrendaResponseDto>.Fail($"Categoría no encontrada para id {request.CategoriaId}.");
        if (await _uow.Repository<ProveedorEntity>().GetById(request.ProveedorId) is null) return ApiResponseDto<PrendaResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");
        if (await _uow.Repository<GeneroEntity>().GetById(request.GeneroId) is null) return ApiResponseDto<PrendaResponseDto>.Fail($"Género no encontrado para id {request.GeneroId}.");

        entity.Nombre = nombre;
        entity.Descripcion = request.Descripcion;
        entity.MarcaId = request.MarcaId;
        entity.CategoriaId = request.CategoriaId;
        entity.ProveedorId = request.ProveedorId;
        entity.GeneroId = request.GeneroId;
        entity.Precio = request.Precio;
        entity.Activo = request.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponseDto<PrendaResponseDto>.Ok(new PrendaResponseDto
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
