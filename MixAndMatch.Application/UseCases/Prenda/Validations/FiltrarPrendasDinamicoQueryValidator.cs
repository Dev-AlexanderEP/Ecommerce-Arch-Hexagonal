using FluentValidation;
using MixAndMatch.Application.UseCases.Prenda.Queries;

namespace MixAndMatch.Application.UseCases.Prenda.Validations;

public class FiltrarPrendasDinamicoQueryValidator : AbstractValidator<FiltrarPrendasDinamicoQuery>
{
    public FiltrarPrendasDinamicoQueryValidator()
    {
        RuleFor(x => x.PrecioMin)
            .GreaterThanOrEqualTo(0).When(x => x.PrecioMin.HasValue)
            .WithMessage("El precio mínimo no puede ser negativo.");

        RuleFor(x => x.PrecioMax)
            .GreaterThanOrEqualTo(0).When(x => x.PrecioMax.HasValue)
            .WithMessage("El precio máximo no puede ser negativo.");

        RuleFor(x => x)
            .Must(x => x.PrecioMin is null || x.PrecioMax is null || x.PrecioMin <= x.PrecioMax)
            .WithMessage("El precio mínimo no puede ser mayor que el precio máximo.")
            .OverridePropertyName("PrecioMin");

        RuleFor(x => x.DescMin)
            .InclusiveBetween(0, 100).When(x => x.DescMin.HasValue)
            .WithMessage("El descuento mínimo debe estar entre 0 y 100.");

        RuleFor(x => x.DescMax)
            .InclusiveBetween(0, 100).When(x => x.DescMax.HasValue)
            .WithMessage("El descuento máximo debe estar entre 0 y 100.");

        RuleFor(x => x)
            .Must(x => x.DescMin is null || x.DescMax is null || x.DescMin <= x.DescMax)
            .WithMessage("El descuento mínimo no puede ser mayor que el descuento máximo.")
            .OverridePropertyName("DescMin");
    }
}
