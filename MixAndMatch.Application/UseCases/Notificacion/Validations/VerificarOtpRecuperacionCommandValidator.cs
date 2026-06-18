using FluentValidation;
using MixAndMatch.Application.UseCases.Notificacion.Commands;

namespace MixAndMatch.Application.UseCases.Notificacion.Validations;

public class VerificarOtpRecuperacionCommandValidator : AbstractValidator<VerificarOtpRecuperacionCommand>
{
    public VerificarOtpRecuperacionCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es obligatorio.");

        RuleFor(x => x.NuevaContrasenia)
            .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}
