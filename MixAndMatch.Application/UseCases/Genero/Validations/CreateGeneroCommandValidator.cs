using FluentValidation;
using MixAndMatch.Application.UseCases.Genero.Commands;

namespace MixAndMatch.Application.UseCases.Genero.Validations;

public class CreateGeneroCommandValidator : AbstractValidator<CreateGeneroCommand>
{
    public CreateGeneroCommandValidator()
    {
        RuleFor(x => x.NomGenero)
            .NotEmpty().WithMessage("El nombre del genero es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre del genero no puede estar vacio.")
            .MaximumLength(100).WithMessage("El nombre del genero no puede superar 100 caracteres.");
    }
}
