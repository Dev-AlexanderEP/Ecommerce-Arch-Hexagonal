using FluentValidation;
using MixAndMatch.Application.UseCases.Pago.Commands;

namespace MixAndMatch.Application.UseCases.Pago.Validations;

public class CreatePagoMercadoPagoCommandValidator : AbstractValidator<CreatePagoMercadoPagoCommand>
{
    public CreatePagoMercadoPagoCommandValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0).WithMessage("El id de la venta debe ser mayor que cero.");

        RuleFor(x => x.MetodoId)
            .GreaterThan(0).WithMessage("El id del metodo de pago debe ser mayor que cero.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El token de la tarjeta es obligatorio.");

        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("El método de pago (paymentMethodId) es obligatorio.");

        RuleFor(x => x.PayerEmail)
            .NotEmpty().WithMessage("El email del pagador es obligatorio.")
            .EmailAddress().WithMessage("El email del pagador no es válido.");

        RuleFor(x => x.Installments)
            .GreaterThanOrEqualTo(1).WithMessage("Las cuotas deben ser al menos 1.");
    }
}
