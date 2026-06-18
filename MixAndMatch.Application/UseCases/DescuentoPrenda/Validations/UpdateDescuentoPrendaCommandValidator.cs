using FluentValidation;
using MixAndMatch.Application.UseCases.DescuentoPrenda.Commands;

namespace MixAndMatch.Application.UseCases.DescuentoPrenda.Validations;

public class UpdateDescuentoPrendaCommandValidator : AbstractValidator<UpdateDescuentoPrendaCommand>
{
    public UpdateDescuentoPrendaCommandValidator()
    {
        RuleFor(x => x.DescuentoPrendaId)
            .GreaterThan(0).WithMessage("El id del descuento de prenda debe ser mayor que cero.");

        RuleFor(x => x.PrendaId)
            .GreaterThan(0).WithMessage("El id de la prenda debe ser mayor que cero.");

        RuleFor(x => x.Porcentaje)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje debe estar entre 0 y 100.");

        RuleFor(x => x.FechaFin)
            .Must((command, fechaFin) => !fechaFin.HasValue || fechaFin.Value >= command.FechaInicio)
            .WithMessage("La fecha fin no puede ser menor a la fecha inicio.");
    }
}
