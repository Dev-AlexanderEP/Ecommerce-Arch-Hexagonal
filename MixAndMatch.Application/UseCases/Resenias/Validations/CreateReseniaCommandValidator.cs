using FluentValidation;
using MixAndMatch.Application.UseCases.Resenias.Commands;

namespace MixAndMatch.Application.UseCases.Resenias.Validations;

public class CreateReseniaCommandValidator : AbstractValidator<CreateReseniaCommand>
{
    public CreateReseniaCommandValidator()
    {
        RuleFor(x => x.PrendaId)
            .GreaterThan(0).WithMessage("El id de la prenda debe ser mayor que cero.");

        RuleFor(x => x.Calificacion)
            .InclusiveBetween(1, 5).WithMessage("La calificacion debe estar entre 1 y 5.");
    }
}
