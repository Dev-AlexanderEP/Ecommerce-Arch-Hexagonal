using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using TallaEntity = MixAndMatch.Domain.Entities.Talla;

namespace MixAndMatch.Application.UseCases.Talla.Queries;

public class GetTallaByIdQuery : IRequest<ApiResponse<TallaResponseDto>>
{
    public required long TallaId { get; set; }
}

public class GetTallaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetTallaByIdQuery, ApiResponse<TallaResponseDto>>
{
    public async Task<ApiResponse<TallaResponseDto>> Handle(GetTallaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<TallaEntity>().GetById(request.TallaId);
        if (entity is null)
            return ApiResponse<TallaResponseDto>.Fail($"Talla no encontrada para id {request.TallaId}.");

        return ApiResponse<TallaResponseDto>.Ok(new TallaResponseDto { Id = entity.Id, NomTalla = entity.NomTalla });
    }
}
