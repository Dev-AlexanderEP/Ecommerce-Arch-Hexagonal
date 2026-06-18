using FluentValidation;
using MixAndMatch.Application.UseCases.Proveedor.Commands;

namespace MixAndMatch.Application.UseCases.Proveedor.Validations;

public class UpdateProveedorCommandValidator : AbstractValidator<UpdateProveedorCommand>
{
    public UpdateProveedorCommandValidator()
    {
        RuleFor(x => x.ProveedorId)
            .GreaterThan(0).WithMessage("El id del proveedor debe ser mayor que cero.");

        RuleFor(x => x.NomProveedor)
            .NotEmpty().WithMessage("El nombre del proveedor es obligatorio.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("El nombre del proveedor no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre del proveedor no puede superar 255 caracteres.");
    }
}
