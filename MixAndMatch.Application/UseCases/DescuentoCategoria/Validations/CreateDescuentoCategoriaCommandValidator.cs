using FluentValidation;
using MixAndMatch.Application.UseCases.DescuentoCategoria.Commands;

namespace MixAndMatch.Application.UseCases.DescuentoCategoria.Validations;

public class CreateDescuentoCategoriaCommandValidator : AbstractValidator<CreateDescuentoCategoriaCommand>
{
    public CreateDescuentoCategoriaCommandValidator()
    {
        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("El id de la categoria debe ser mayor que cero.");

        RuleFor(x => x.Porcentaje)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje debe estar entre 0 y 100.");

        RuleFor(x => x.FechaFin)
            .Must((command, fechaFin) => !fechaFin.HasValue || fechaFin.Value >= command.FechaInicio)
            .WithMessage("La fecha fin no puede ser menor a la fecha inicio.");
    }
}
