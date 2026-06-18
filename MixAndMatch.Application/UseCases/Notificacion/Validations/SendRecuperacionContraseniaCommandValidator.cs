using FluentValidation;
using MixAndMatch.Application.UseCases.Notificacion.Commands;

namespace MixAndMatch.Application.UseCases.Notificacion.Validations;

public class SendRecuperacionContraseniaCommandValidator : AbstractValidator<SendRecuperacionContraseniaCommand>
{
    public SendRecuperacionContraseniaCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(255);
    }
}
