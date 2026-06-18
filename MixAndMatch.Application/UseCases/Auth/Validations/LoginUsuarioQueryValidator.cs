using FluentValidation;
using MixAndMatch.Application.UseCases.Auth.Queries;

namespace MixAndMatch.Application.UseCases.Auth.Validations;

public class LoginUsuarioQueryValidator : AbstractValidator<LoginUsuarioQuery>
{
    public LoginUsuarioQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Contrasenia)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
