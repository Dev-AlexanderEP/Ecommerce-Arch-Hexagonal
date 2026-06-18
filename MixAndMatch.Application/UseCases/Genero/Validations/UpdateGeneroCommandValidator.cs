using FluentValidation;
using MixAndMatch.Application.UseCases.Genero.Commands;

namespace MixAndMatch.Application.UseCases.Genero.Validations;

public class UpdateGeneroCommandValidator : AbstractValidator<UpdateGeneroCommand>
{
    public UpdateGeneroCommandValidator()
    {
        RuleFor(x => x.GeneroId)
            .GreaterThan(0).WithMessage("El id del genero debe ser mayor que cero.");

        RuleFor(x => x.NomGenero)
            .NotEmpty().WithMessage("El nombre del genero es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre del genero no puede estar vacio.")
            .MaximumLength(100).WithMessage("El nombre del genero no puede superar 100 caracteres.");
    }
}
