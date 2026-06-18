using FluentValidation;
using MixAndMatch.Application.UseCases.Auth.Commands;

namespace MixAndMatch.Application.UseCases.Auth.Validations;

public class RegisterUsuarioCommandValidator : AbstractValidator<RegisterUsuarioCommand>
{
    public RegisterUsuarioCommandValidator()
    {
        RuleFor(x => x.NombreUsuario)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MaximumLength(255);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(255);

        RuleFor(x => x.Contrasenia)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}
