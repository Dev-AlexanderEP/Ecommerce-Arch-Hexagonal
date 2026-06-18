using FluentValidation;
using MixAndMatch.Application.UseCases.CarritoItem.Commands;

namespace MixAndMatch.Application.UseCases.CarritoItem.Validations;

public class UpdateCarritoItemCommandValidator : AbstractValidator<UpdateCarritoItemCommand>
{
    public UpdateCarritoItemCommandValidator()
    {
        RuleFor(x => x.CarritoItemId)
            .GreaterThan(0).WithMessage("El id del item debe ser mayor que cero.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor que cero.");
    }
}
