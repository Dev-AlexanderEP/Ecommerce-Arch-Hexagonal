using FluentValidation;
using MixAndMatch.Application.UseCases.Auth.Commands;

namespace MixAndMatch.Application.UseCases.Auth.Validations;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.ContraseniaActual)
            .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

        RuleFor(x => x.ContraseniaNueva)
            .NotEmpty().WithMessage("La contraseña nueva es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña nueva debe tener al menos 8 caracteres.")
            .NotEqual(x => x.ContraseniaActual).WithMessage("La contraseña nueva debe ser distinta a la actual.");
    }
}
