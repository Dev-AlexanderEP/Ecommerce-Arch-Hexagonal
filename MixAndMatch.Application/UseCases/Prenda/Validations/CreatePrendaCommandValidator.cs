using FluentValidation;
using MixAndMatch.Application.UseCases.Prenda.Commands;

namespace MixAndMatch.Application.UseCases.Prenda.Validations;

public class CreatePrendaCommandValidator : AbstractValidator<CreatePrendaCommand>
{
    public CreatePrendaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la prenda es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre de la prenda no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre de la prenda no puede superar 255 caracteres.");

        RuleFor(x => x.MarcaId)
            .GreaterThan(0).WithMessage("El id de la marca debe ser mayor que cero.");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("El id de la categoria debe ser mayor que cero.");

        RuleFor(x => x.ProveedorId)
            .GreaterThan(0).WithMessage("El id del proveedor debe ser mayor que cero.");

        RuleFor(x => x.GeneroId)
            .GreaterThan(0).WithMessage("El id del genero debe ser mayor que cero.");

        RuleFor(x => x.Precio)
            .GreaterThanOrEqualTo(0m).WithMessage("El precio no puede ser negativo.");
    }
}
