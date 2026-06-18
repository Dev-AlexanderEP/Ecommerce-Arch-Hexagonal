using FluentValidation;
using MixAndMatch.Application.UseCases.Pago.Commands;

namespace MixAndMatch.Application.UseCases.Pago.Validations;

public class CreatePagoCommandValidator : AbstractValidator<CreatePagoCommand>
{
    public CreatePagoCommandValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0).WithMessage("El id de la venta debe ser mayor que cero.");

        RuleFor(x => x.MetodoId)
            .GreaterThan(0).WithMessage("El id del metodo de pago debe ser mayor que cero.");
    }
}
