using MediatR;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;

namespace MixAndMatch.Application.UseCases.Marca.Queries;

public class GetMarcaByIdQuery : IRequest<ApiResponseDto<MarcaResponseDto>>
{
    public required long MarcaId { get; set; }
}

public class GetMarcaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetMarcaByIdQuery, ApiResponseDto<MarcaResponseDto>>
{
    public async Task<ApiResponseDto<MarcaResponseDto>> Handle(GetMarcaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<MarcaEntity>().GetById(request.MarcaId);
        if (entity is null)
        {
            return ApiResponseDto<MarcaResponseDto>.Fail($"Marca no encontrada para id {request.MarcaId}.");
        }

        return ApiResponseDto<MarcaResponseDto>.Ok(new MarcaResponseDto
        {
            Id = entity.Id,
            NomMarca = entity.NomMarca
        });
    }
}
