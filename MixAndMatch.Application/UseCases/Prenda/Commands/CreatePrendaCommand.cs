using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using CategoriaEntity = MixAndMatch.Domain.Entities.Categoria;
using GeneroEntity = MixAndMatch.Domain.Entities.Genero;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;
using ProveedorEntity = MixAndMatch.Domain.Entities.Proveedor;

namespace MixAndMatch.Application.UseCases.Prenda.Commands;

public class CreatePrendaCommand : IRequest<ApiResponse<PrendaResponseDto>>
{
    public required string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public required long MarcaId { get; set; }
    public required long CategoriaId { get; set; }
    public required long ProveedorId { get; set; }
    public required long GeneroId { get; set; }
    public required decimal Precio { get; set; }
    public required bool Activo { get; set; }
}

public class CreatePrendaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreatePrendaCommand, ApiResponse<PrendaResponseDto>>
{
    public async Task<ApiResponse<PrendaResponseDto>> Handle(CreatePrendaCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.Nombre ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<PrendaResponseDto>.Fail("El nombre de la prenda es obligatorio.");
        }

        if (request.Precio < 0)
        {
            return ApiResponse<PrendaResponseDto>.Fail("El precio no puede ser negativo.");
        }

        var marca = await _uow.Repository<MarcaEntity>().GetById(request.MarcaId);
        if (marca is null) return ApiResponse<PrendaResponseDto>.Fail($"Marca no encontrada para id {request.MarcaId}.");

        var categoria = await _uow.Repository<CategoriaEntity>().GetById(request.CategoriaId);
        if (categoria is null) return ApiResponse<PrendaResponseDto>.Fail($"CategorÃ­a no encontrada para id {request.CategoriaId}.");

        var proveedor = await _uow.Repository<ProveedorEntity>().GetById(request.ProveedorId);
        if (proveedor is null) return ApiResponse<PrendaResponseDto>.Fail($"Proveedor no encontrado para id {request.ProveedorId}.");

        var genero = await _uow.Repository<GeneroEntity>().GetById(request.GeneroId);
        if (genero is null) return ApiResponse<PrendaResponseDto>.Fail($"GÃ©nero no encontrado para id {request.GeneroId}.");

        var entity = new PrendaEntity
        {
            Nombre = nombre,
            Descripcion = request.Descripcion,
            MarcaId = request.MarcaId,
            CategoriaId = request.CategoriaId,
            ProveedorId = request.ProveedorId,
            GeneroId = request.GeneroId,
            Precio = request.Precio,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow
        };

        var repo = _uow.Repository<PrendaEntity>();
        await repo.Add(entity);
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
