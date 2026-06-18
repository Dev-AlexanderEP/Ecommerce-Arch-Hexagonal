using FluentValidation;
using MixAndMatch.Application.UseCases.Marca.Commands;

namespace MixAndMatch.Application.UseCases.Marca.Validations;

public class CreateMarcaCommandValidator : AbstractValidator<CreateMarcaCommand>
{
    public CreateMarcaCommandValidator()
    {
        RuleFor(x => x.NomMarca)
            .NotEmpty().WithMessage("El nombre de la marca es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre de la marca no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre de la marca no puede superar 255 caracteres.");
    }
}
