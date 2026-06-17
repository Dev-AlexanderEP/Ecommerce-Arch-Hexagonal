using FluentValidation;
using MixAndMatch.Application.UseCases.Categoria.Commands;

namespace MixAndMatch.Application.UseCases.Categoria.Validations;

public class UpdateCategoriaCommandValidator : AbstractValidator<UpdateCategoriaCommand>
{
    public UpdateCategoriaCommandValidator()
    {
        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("El id de la categoria debe ser mayor que cero.");

        RuleFor(x => x.NomCategoria)
            .NotEmpty().WithMessage("El nombre de la categoria es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre de la categoria no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre de la categoria no puede superar 255 caracteres.");
    }
}
