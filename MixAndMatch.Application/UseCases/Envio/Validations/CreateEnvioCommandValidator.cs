using FluentValidation;
using MixAndMatch.Application.UseCases.Envio.Commands;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Application.UseCases.Envio.Validations;

public class CreateEnvioCommandValidator : AbstractValidator<CreateEnvioCommand>
{
    public CreateEnvioCommandValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0).WithMessage("El id de la venta debe ser mayor que cero.");

        RuleFor(x => x.DatosEnvioId)
            .GreaterThan(0).WithMessage("El id de los datos de envio debe ser mayor que cero.");

        RuleFor(x => x.CostoEnvio)
            .GreaterThanOrEqualTo(0m).WithMessage("El costo de envio no puede ser negativo.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(SerEstadoValido)
            .WithMessage(x => $"Estado invalido: {x.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoEnvio>())}.");

        RuleFor(x => x.MetodoEnvio)
            .NotEmpty().WithMessage("El metodo de envio es obligatorio.")
            .MaximumLength(50).WithMessage("El metodo de envio no puede superar 50 caracteres.");

        RuleFor(x => x.TrackingNumber)
            .MaximumLength(100).WithMessage("El tracking number no puede superar 100 caracteres.");
    }

    private static bool SerEstadoValido(string estado) =>
        Enum.TryParse<EstadoEnvio>(estado, ignoreCase: true, out _);
}
