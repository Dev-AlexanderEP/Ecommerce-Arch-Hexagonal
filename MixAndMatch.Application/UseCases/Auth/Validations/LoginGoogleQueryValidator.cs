using FluentValidation;
using MixAndMatch.Application.UseCases.Auth.Queries;

namespace MixAndMatch.Application.UseCases.Auth.Validations;

public class LoginGoogleQueryValidator : AbstractValidator<LoginGoogleQuery>
{
    public LoginGoogleQueryValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("El idToken de Google es obligatorio.");
    }
}
