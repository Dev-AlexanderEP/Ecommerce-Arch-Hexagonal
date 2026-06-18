using FluentValidation;
using MixAndMatch.Application.UseCases.DescuentoCodigo.Commands;

namespace MixAndMatch.Application.UseCases.DescuentoCodigo.Validations;

public class CreateDescuentoCodigoCommandValidator : AbstractValidator<CreateDescuentoCodigoCommand>
{
    public CreateDescuentoCodigoCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .MaximumLength(50).WithMessage("El código no puede superar 50 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(255).WithMessage("La descripción no puede superar 255 caracteres.");

        RuleFor(x => x.Porcentaje)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje debe estar entre 0 y 100.");

        RuleFor(x => x.UsoMaximo)
            .GreaterThan(0).WithMessage("El uso máximo debe ser mayor a 0.");

        RuleFor(x => x.FechaFin)
            .Must((command, fechaFin) => !fechaFin.HasValue || fechaFin.Value >= command.FechaInicio)
            .WithMessage("La fecha fin no puede ser menor a la fecha inicio.");
    }
}
