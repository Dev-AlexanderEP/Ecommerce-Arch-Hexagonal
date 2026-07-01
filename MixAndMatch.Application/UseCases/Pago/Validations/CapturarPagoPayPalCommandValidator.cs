using FluentValidation;
using MixAndMatch.Application.UseCases.Pago.Commands;

namespace MixAndMatch.Application.UseCases.Pago.Validations;

public class CapturarPagoPayPalCommandValidator : AbstractValidator<CapturarPagoPayPalCommand>
{
    public CapturarPagoPayPalCommandValidator()
    {
        RuleFor(x => x.PagoId)
            .GreaterThan(0).WithMessage("El id del pago debe ser mayor que cero.");

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El id de la orden de PayPal es obligatorio.");
    }
}
