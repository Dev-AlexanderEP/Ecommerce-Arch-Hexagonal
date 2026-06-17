using FluentValidation;
using MixAndMatch.Application.UseCases.VentasDetalle.Commands;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Validations;

public class UpdateVentasDetalleCommandValidator : AbstractValidator<UpdateVentasDetalleCommand>
{
    public UpdateVentasDetalleCommandValidator()
    {
        RuleFor(x => x.VentasDetalleId)
            .GreaterThan(0).WithMessage("El id del detalle de venta debe ser mayor que cero.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThan(0m).WithMessage("El precio unitario debe ser mayor a 0.");
    }
}
