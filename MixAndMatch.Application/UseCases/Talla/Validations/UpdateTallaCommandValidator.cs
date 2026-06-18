using FluentValidation;
using MixAndMatch.Application.UseCases.Talla.Commands;

namespace MixAndMatch.Application.UseCases.Talla.Validations;

public class UpdateTallaCommandValidator : AbstractValidator<UpdateTallaCommand>
{
    public UpdateTallaCommandValidator()
    {
        RuleFor(x => x.TallaId)
            .GreaterThan(0).WithMessage("El id de la talla debe ser mayor que cero.");

        RuleFor(x => x.NomTalla)
            .NotEmpty().WithMessage("El nombre de la talla es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre de la talla no puede estar vacio.")
            .MaximumLength(20).WithMessage("El nombre de la talla no puede superar 20 caracteres.");
    }
}
