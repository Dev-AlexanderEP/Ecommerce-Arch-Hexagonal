using FluentValidation;
using MixAndMatch.Application.UseCases.MetodoPago.Commands;

namespace MixAndMatch.Application.UseCases.MetodoPago.Validations;

public class CreateMetodoPagoCommandValidator : AbstractValidator<CreateMetodoPagoCommand>
{
    public CreateMetodoPagoCommandValidator()
    {
        RuleFor(x => x.TipoPago)
            .NotEmpty().WithMessage("El tipo de pago es obligatorio.")
            .MaximumLength(50).WithMessage("El tipo de pago no puede superar 50 caracteres.");
    }
}
