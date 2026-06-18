using FluentValidation;
using MixAndMatch.Application.UseCases.Notificacion.Commands;

namespace MixAndMatch.Application.UseCases.Notificacion.Validations;

public class SendWelcomeEmailCommandValidator : AbstractValidator<SendWelcomeEmailCommand>
{
    public SendWelcomeEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(255);

        RuleFor(x => x.NombreUsuario)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MaximumLength(255);
    }
}
