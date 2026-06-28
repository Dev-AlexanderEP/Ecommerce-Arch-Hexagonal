using FluentValidation;
using MixAndMatch.Application.UseCases.PrendaTalla.Commands;

namespace MixAndMatch.Application.UseCases.PrendaTalla.Validations;

public class SumarStockCommandValidator : AbstractValidator<SumarStockCommand>
{
    public SumarStockCommandValidator()
    {
        RuleFor(x => x.PrendaId).GreaterThan(0).WithMessage("El id de la prenda debe ser mayor que cero.");
        RuleFor(x => x.TallaId).GreaterThan(0).WithMessage("El id de la talla debe ser mayor que cero.");
        RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");
    }
}
