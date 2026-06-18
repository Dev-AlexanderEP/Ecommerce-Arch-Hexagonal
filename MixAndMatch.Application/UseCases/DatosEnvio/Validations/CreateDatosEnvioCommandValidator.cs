using FluentValidation;
using MixAndMatch.Application.UseCases.DatosEnvio.Commands;

namespace MixAndMatch.Application.UseCases.DatosEnvio.Validations;

public class CreateDatosEnvioCommandValidator : AbstractValidator<CreateDatosEnvioCommand>
{
    public CreateDatosEnvioCommandValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().WithMessage("Los nombres son obligatorios.").MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().WithMessage("Los apellidos son obligatorios.").MaximumLength(100);
        RuleFor(x => x.Dni).NotEmpty().WithMessage("El DNI es obligatorio.").MaximumLength(20);
        RuleFor(x => x.Departamento).NotEmpty().WithMessage("El departamento es obligatorio.").MaximumLength(100);
        RuleFor(x => x.Provincia).NotEmpty().WithMessage("La provincia es obligatoria.").MaximumLength(100);
        RuleFor(x => x.Distrito).NotEmpty().WithMessage("El distrito es obligatorio.").MaximumLength(100);
        RuleFor(x => x.Calle).MaximumLength(255);
        RuleFor(x => x.Detalle).NotEmpty().WithMessage("El detalle es obligatorio.").MaximumLength(255);
        RuleFor(x => x.Telefono).NotEmpty().WithMessage("El telefono es obligatorio.").MaximumLength(20);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato valido.")
            .MaximumLength(100);
    }
}
