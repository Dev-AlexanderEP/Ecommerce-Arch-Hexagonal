using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;

namespace MixAndMatch.Application.UseCases.Marca.Commands;

public class UpdateMarcaCommand : IRequest<ApiResponse<MarcaResponseDto>>
{
    public required long MarcaId { get; set; }
    public required string NomMarca { get; set; }
}

public class UpdateMarcaCommandHandler(IUnitOfWork _uow) : IRequestHandler<UpdateMarcaCommand, ApiResponse<MarcaResponseDto>>
{
    public async Task<ApiResponse<MarcaResponseDto>> Handle(UpdateMarcaCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<MarcaEntity>();
        var entity = await repo.GetById(request.MarcaId);
        if (entity is null)
        {
            return ApiResponse<MarcaResponseDto>.Fail($"Marca no encontrada para id {request.MarcaId}.");
        }

        var nombre = (request.NomMarca ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<MarcaResponseDto>.Fail("El nombre de la marca es obligatorio.");
        }

        var items = await repo.GetAll();
        if (items.Any(x => x.Id != request.MarcaId && x.NomMarca == nombre))
        {
            return ApiResponse<MarcaResponseDto>.Fail("La marca ya existe.");
        }

        entity.NomMarca = nombre;
        await repo.Update(entity);
        await _uow.Complete();

        return ApiResponse<MarcaResponseDto>.Ok(new MarcaResponseDto
        {
            Id = entity.Id,
            NomMarca = entity.NomMarca
        });
    }
}
