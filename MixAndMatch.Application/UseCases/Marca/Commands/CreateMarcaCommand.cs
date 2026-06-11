using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;
using PrendaEntity = MixAndMatch.Domain.Entities.Prenda;

namespace MixAndMatch.Application.UseCases.Marca.Commands;

public class CreateMarcaCommand : IRequest<ApiResponse<MarcaResponseDto>>
{
    public required string NomMarca { get; set; }
}

public class CreateMarcaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateMarcaCommand, ApiResponse<MarcaResponseDto>>
{
    public async Task<ApiResponse<MarcaResponseDto>> Handle(CreateMarcaCommand request, CancellationToken cancellationToken)
    {
        var nombre = (request.NomMarca ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ApiResponse<MarcaResponseDto>.Fail("El nombre de la marca es obligatorio.");
        }

        var repo = _uow.Repository<MarcaEntity>();
        var items = await repo.GetAll();
        if (items.Any(x => x.NomMarca == nombre))
        {
            return ApiResponse<MarcaResponseDto>.Fail("La marca ya existe.");
        }

        var entity = new MarcaEntity { NomMarca = nombre };
        await repo.Add(entity);
        await _uow.Complete();

        return ApiResponse<MarcaResponseDto>.Ok(new MarcaResponseDto
        {
            Id = entity.Id,
            NomMarca = entity.NomMarca
        });
    }
}
