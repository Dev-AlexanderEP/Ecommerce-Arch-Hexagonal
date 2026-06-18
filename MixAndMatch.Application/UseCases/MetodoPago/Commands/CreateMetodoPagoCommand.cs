using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using MetodoPagoEntity = MixAndMatch.Domain.Entities.MetodoPago;

namespace MixAndMatch.Application.UseCases.MetodoPago.Commands;

public class CreateMetodoPagoCommand : IRequest<ApiResponse<MetodoPagoResponseDto>>
{
    public required string TipoPago { get; set; }
}

public class CreateMetodoPagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<CreateMetodoPagoCommand, ApiResponse<MetodoPagoResponseDto>>
{
    public async Task<ApiResponse<MetodoPagoResponseDto>> Handle(CreateMetodoPagoCommand request, CancellationToken cancellationToken)
    {
        var tipo = request.TipoPago.Trim();

        if (await _uow.MetodoPagos.ExisteConTipo(tipo))
        {
            return ApiResponse<MetodoPagoResponseDto>.Fail("El metodo de pago ya existe.", ErrorType.Conflict);
        }

        var entity = new MetodoPagoEntity
        {
            TipoPago = tipo,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.MetodoPagos.Add(entity);
        await _uow.Complete();

        return ApiResponse<MetodoPagoResponseDto>.Created(new MetodoPagoResponseDto
        {
            Id = entity.Id,
            TipoPago = entity.TipoPago,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
