using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Queries;

public class GetTallaByIdQuery : IRequest<ApiResponseDto<TallaResponseDto>>
{
    public required long TallaId { get; set; }
}

public class GetTallaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetTallaByIdQuery, ApiResponseDto<TallaResponseDto>>
{
    public async Task<ApiResponseDto<TallaResponseDto>> Handle(GetTallaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<TallaEntity>().GetById(request.TallaId);
        if (entity is null)
            return ApiResponseDto<TallaResponseDto>.Fail($"Talla no encontrada para id {request.TallaId}.");

        return ApiResponseDto<TallaResponseDto>.Ok(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
