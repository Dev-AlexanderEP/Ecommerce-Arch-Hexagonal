using FluentValidation;
using MixAndMatch.Application.UseCases.Marca.Commands;

namespace MixAndMatch.Application.UseCases.Marca.Validations;

public class UpdateMarcaCommandValidator : AbstractValidator<UpdateMarcaCommand>
{
    public UpdateMarcaCommandValidator()
    {
        RuleFor(x => x.MarcaId)
            .GreaterThan(0).WithMessage("El id de la marca debe ser mayor que cero.");

        RuleFor(x => x.NomMarca)
            .NotEmpty().WithMessage("El nombre de la marca es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre de la marca no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre de la marca no puede superar 255 caracteres.");
    }
}
