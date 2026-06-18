using FluentValidation;
using MixAndMatch.Application.UseCases.MetodoPago.Commands;

namespace MixAndMatch.Application.UseCases.MetodoPago.Validations;

public class UpdateMetodoPagoCommandValidator : AbstractValidator<UpdateMetodoPagoCommand>
{
    public UpdateMetodoPagoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id del metodo de pago debe ser mayor que cero.");

        RuleFor(x => x.TipoPago)
            .NotEmpty().WithMessage("El tipo de pago es obligatorio.")
            .MaximumLength(50).WithMessage("El tipo de pago no puede superar 50 caracteres.");
    }
}
