using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Commands;

public class CreateTallaCommand : IRequest<ApiResponse<TallaResponseDto>>
{
    public required string NomTalla { get; set; }
}

public class CreateTallaCommandHandler(IUnitOfWork _uow) : IRequestHandler<CreateTallaCommand, ApiResponse<TallaResponseDto>>
{
    public async Task<ApiResponse<TallaResponseDto>> Handle(CreateTallaCommand request, CancellationToken cancellationToken)
    {
        var nombre = request.NomTalla.Trim();

        if (await _uow.Tallas.ExisteConNombre(nombre))
        {
            return ApiResponse<TallaResponseDto>.Fail("La talla ya existe.", ErrorType.Conflict);
        }

        var entity = new TallaEntity { NomTalla = nombre };
        await _uow.Tallas.Add(entity);
        await _uow.Complete();

        return ApiResponse<TallaResponseDto>.Created(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
