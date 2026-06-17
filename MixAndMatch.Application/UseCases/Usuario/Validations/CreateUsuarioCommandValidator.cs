using FluentValidation;
using MixAndMatch.Application.UseCases.Usuario.Commands;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Application.UseCases.Usuario.Validations;

public class CreateUsuarioCommandValidator : AbstractValidator<CreateUsuarioCommand>
{
    public CreateUsuarioCommandValidator()
    {
        RuleFor(x => x.NombreUsuario)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MaximumLength(255).WithMessage("El nombre de usuario no puede superar 255 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato valido.")
            .MaximumLength(255).WithMessage("El email no puede superar 255 caracteres.");

        RuleFor(x => x.Contrasenia)
            .NotEmpty().WithMessage("La contrasenia es obligatoria.");

        RuleFor(x => x.Rol)
            .Must(SerRolValido)
            .WithMessage(x => $"Rol invalido: {x.Rol}. Permitidos: {string.Join(", ", Enum.GetNames<RolUsuario>())}.")
            .When(x => !string.IsNullOrWhiteSpace(x.Rol));
    }

    private static bool SerRolValido(string? rol) =>
        Enum.TryParse<RolUsuario>(rol, ignoreCase: true, out _);
}
