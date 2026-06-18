using FluentValidation;
using MixAndMatch.Application.UseCases.Categoria.Commands;

namespace MixAndMatch.Application.UseCases.Categoria.Validations;

public class CreateCategoriaCommandValidator : AbstractValidator<CreateCategoriaCommand>
{
    public CreateCategoriaCommandValidator()
    {
        RuleFor(x => x.NomCategoria)
            .NotEmpty().WithMessage("El nombre de la categoria es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre de la categoria no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre de la categoria no puede superar 255 caracteres.");
    }
}
