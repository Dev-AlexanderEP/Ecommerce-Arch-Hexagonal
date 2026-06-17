using FluentValidation;
using MixAndMatch.Application.UseCases.CarritoItem.Commands;

namespace MixAndMatch.Application.UseCases.CarritoItem.Validations;

public class CreateCarritoItemCommandValidator : AbstractValidator<CreateCarritoItemCommand>
{
    public CreateCarritoItemCommandValidator()
    {
        RuleFor(x => x.CarritoId)
            .GreaterThan(0).WithMessage("El id del carrito debe ser mayor que cero.");

        RuleFor(x => x.PrendaTallaId)
            .GreaterThan(0).WithMessage("El id de la prenda-talla debe ser mayor que cero.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor que cero.");
    }
}
