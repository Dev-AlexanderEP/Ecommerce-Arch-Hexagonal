using FluentValidation;
using MixAndMatch.Application.UseCases.PrendaImagen.Commands;

namespace MixAndMatch.Application.UseCases.PrendaImagen.Validations;

public class UploadPrendaImagenCommandValidator : AbstractValidator<UploadPrendaImagenCommand>
{
    public UploadPrendaImagenCommandValidator()
    {
        RuleFor(x => x.Contenido)
            .NotNull().WithMessage("El archivo es requerido.");

        RuleFor(x => x.NombreArchivo)
            .NotEmpty().WithMessage("El nombre del archivo es requerido.");
    }
}
