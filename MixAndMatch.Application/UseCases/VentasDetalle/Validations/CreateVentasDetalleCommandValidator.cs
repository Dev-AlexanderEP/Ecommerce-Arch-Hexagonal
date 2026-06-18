using FluentValidation;
using MixAndMatch.Application.UseCases.VentasDetalle.Commands;

namespace MixAndMatch.Application.UseCases.VentasDetalle.Validations;

public class CreateVentasDetalleCommandValidator : AbstractValidator<CreateVentasDetalleCommand>
{
    public CreateVentasDetalleCommandValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0).WithMessage("El id de la venta debe ser mayor que cero.");

        RuleFor(x => x.PrendaTallaId)
            .GreaterThan(0).WithMessage("El id de la prenda-talla debe ser mayor que cero.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThan(0m).WithMessage("El precio unitario debe ser mayor a 0.");
    }
}
