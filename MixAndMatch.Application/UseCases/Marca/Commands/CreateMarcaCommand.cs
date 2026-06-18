using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;

namespace MixAndMatch.Application.UseCases.Marca.Commands;

public class CreateMarcaCommand : IRequest<ApiResponse<MarcaResponseDto>>
{
    public required string NomMarca { get; set; }
}

public class CreateMarcaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateMarcaCommand, ApiResponse<MarcaResponseDto>>
{
    public async Task<ApiResponse<MarcaResponseDto>> Handle(CreateMarcaCommand request, CancellationToken cancellationToken)
    {
        var nombre = request.NomMarca.Trim();

        if (await _uow.Marcas.ExisteConNombre(nombre))
        {
            return ApiResponse<MarcaResponseDto>.Fail("La marca ya existe.", ErrorType.Conflict);
        }

        var entity = new MarcaEntity { NomMarca = nombre };
        await _uow.Marcas.Add(entity);
        await _uow.Complete();

        return ApiResponse<MarcaResponseDto>.Created(new MarcaResponseDto
        {
            Id = entity.Id,
            NomMarca = entity.NomMarca
        });
    }
}
