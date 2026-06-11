using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Ports.IRepositories;
using MarcaEntity = MixAndMatch.Domain.Entities.Marca;

namespace MixAndMatch.Application.UseCases.Marca.Queries;

public class GetMarcaByIdQuery : IRequest<ApiResponse<MarcaResponseDto>>
{
    public required long MarcaId { get; set; }
}

public class GetMarcaByIdQueryHandler(IUnitOfWork _uow) : IRequestHandler<GetMarcaByIdQuery, ApiResponse<MarcaResponseDto>>
{
    public async Task<ApiResponse<MarcaResponseDto>> Handle(GetMarcaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.Repository<MarcaEntity>().GetById(request.MarcaId);
        if (entity is null)
        {
            return ApiResponse<MarcaResponseDto>.Fail($"Marca no encontrada para id {request.MarcaId}.");
        }

        return ApiResponse<MarcaResponseDto>.Ok(new MarcaResponseDto
        {
            Id = entity.Id,
            NomMarca = entity.NomMarca
        });
    }
}
