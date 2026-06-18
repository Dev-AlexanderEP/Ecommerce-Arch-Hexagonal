using FluentValidation;
using MixAndMatch.Application.UseCases.PrendaImagen.Commands;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Validations;

public class CreatePrendaImagenCommandValidator : AbstractValidator<CreatePrendaImagenCommand>
{
    public CreatePrendaImagenCommandValidator()
    {
        RuleFor(x => x.PrendaId)
            .GreaterThan(0).WithMessage("El id de la prenda debe ser mayor que cero.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("El tipo de imagen es obligatorio.")
            .Must(SerTipoValido)
            .WithMessage(x => $"Tipo de imagen invalido: {x.Tipo}. Permitidos: {string.Join(", ", Enum.GetNames<TipoImagen>())}.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("La URL de la imagen es obligatoria.")
            .Must(u => !string.IsNullOrWhiteSpace(u)).WithMessage("La URL de la imagen no puede estar vacia.")
            .MaximumLength(500).WithMessage("La URL no puede superar 500 caracteres.");
    }

    private static bool SerTipoValido(string tipo) =>
        Enum.TryParse<TipoImagen>(tipo, ignoreCase: true, out _);
}
