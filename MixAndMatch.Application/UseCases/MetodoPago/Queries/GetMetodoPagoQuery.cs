using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.MetodoPago.Queries;

public class GetMetodoPagoByIdQuery : IRequest<ApiResponse<MetodoPagoResponseDto>>
{
    public required long Id { get; set; }
}

public class GetMetodoPagoByIdQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetMetodoPagoByIdQuery, ApiResponse<MetodoPagoResponseDto>>
{
    public async Task<ApiResponse<MetodoPagoResponseDto>> Handle(GetMetodoPagoByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _uow.MetodoPagos.GetById(request.Id);
        if (entity is null)
        {
            return ApiResponse<MetodoPagoResponseDto>.Fail($"Metodo de pago no encontrado para id {request.Id}.");
        }

        return ApiResponse<MetodoPagoResponseDto>.Ok(new MetodoPagoResponseDto
        {
            Id = entity.Id,
            TipoPago = entity.TipoPago,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
