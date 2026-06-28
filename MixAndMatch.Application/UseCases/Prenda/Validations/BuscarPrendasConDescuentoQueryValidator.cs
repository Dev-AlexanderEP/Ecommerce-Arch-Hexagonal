using FluentValidation;
using MixAndMatch.Application.UseCases.Prenda.Queries;

namespace MixAndMatch.Application.UseCases.Prenda.Validations;

public class BuscarPrendasConDescuentoQueryValidator : AbstractValidator<BuscarPrendasConDescuentoQuery>
{
    public BuscarPrendasConDescuentoQueryValidator()
    {
        RuleFor(x => x.Nombre)
            .MaximumLength(255).When(x => x.Nombre is not null)
            .WithMessage("El nombre de búsqueda no puede superar 255 caracteres.");

        RuleFor(x => x.Categoria)
            .MaximumLength(100).When(x => x.Categoria is not null)
            .WithMessage("La categoría no puede superar 100 caracteres.");

        RuleFor(x => x.Genero)
            .MaximumLength(100).When(x => x.Genero is not null)
            .WithMessage("El género no puede superar 100 caracteres.");
    }
}
