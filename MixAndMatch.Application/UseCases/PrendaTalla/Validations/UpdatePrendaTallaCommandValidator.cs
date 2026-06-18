using FluentValidation;
using MixAndMatch.Application.UseCases.PrendaTalla.Commands;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Validations;

public class UpdatePrendaTallaCommandValidator : AbstractValidator<UpdatePrendaTallaCommand>
{
    public UpdatePrendaTallaCommandValidator()
    {
        RuleFor(x => x.PrendaTallaId)
            .GreaterThan(0).WithMessage("El id de la prenda-talla debe ser mayor que cero.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
    }
}
