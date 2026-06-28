using FluentValidation;
using MixAndMatch.Application.UseCases.PrendaTalla.Commands;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Validations;

public class RestarUnoStockCommandValidator : AbstractValidator<RestarUnoStockCommand>
{
    public RestarUnoStockCommandValidator()
    {
        RuleFor(x => x.PrendaId).GreaterThan(0).WithMessage("El id de la prenda debe ser mayor que cero.");
        RuleFor(x => x.TallaId).GreaterThan(0).WithMessage("El id de la talla debe ser mayor que cero.");
    }
}
