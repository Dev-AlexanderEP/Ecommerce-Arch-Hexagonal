using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

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

        await _uow.Prendas.Add(entity);
        await _uow.Complete();

        return ApiResponse<PrendaResponseDto>.Created(new PrendaResponseDto
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
