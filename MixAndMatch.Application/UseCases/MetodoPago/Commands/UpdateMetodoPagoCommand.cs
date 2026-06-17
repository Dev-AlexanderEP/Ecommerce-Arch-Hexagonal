using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using MetodoPagoEntity = MixAndMatch.Domain.Entities.MetodoPago;

namespace MixAndMatch.Application.UseCases.MetodoPago.Commands;

public class UpdateMetodoPagoCommand : IRequest<ApiResponse<MetodoPagoResponseDto>>
{
    [JsonIgnore]   // lo asigna el controller desde la ruta
    public long Id { get; set; }
    public required string TipoPago { get; set; }
}

public class UpdateMetodoPagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<UpdateMetodoPagoCommand, ApiResponse<MetodoPagoResponseDto>>
{
    public async Task<ApiResponse<MetodoPagoResponseDto>> Handle(UpdateMetodoPagoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.MetodoPagos.GetById(request.Id);
        if (entity is null)
        {
            return ApiResponse<MetodoPagoResponseDto>.Fail($"Metodo de pago no encontrado para id {request.Id}.");
        }

        var tipo = request.TipoPago.Trim();
        if (await _uow.MetodoPagos.ExisteConTipo(tipo, request.Id))
        {
            return ApiResponse<MetodoPagoResponseDto>.Fail("El metodo de pago ya existe.", ErrorType.Conflict);
        }

        entity.TipoPago = tipo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.MetodoPagos.Update(entity);
        await _uow.Complete();

        return ApiResponse<MetodoPagoResponseDto>.Ok(new MetodoPagoResponseDto
        {
            Id = entity.Id,
            TipoPago = entity.TipoPago,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }
}
