using FluentValidation;
using MixAndMatch.Application.UseCases.Pago.Commands;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Application.UseCases.Pago.Validations;

public class UpdatePagoCommandValidator : AbstractValidator<UpdatePagoCommand>
{
    public UpdatePagoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id del pago debe ser mayor que cero.");

        RuleFor(x => x.VentaId)
            .GreaterThan(0).WithMessage("El id de la venta debe ser mayor que cero.");

        RuleFor(x => x.MetodoId)
            .GreaterThan(0).WithMessage("El id del metodo de pago debe ser mayor que cero.");

        RuleFor(x => x.Monto)
            .GreaterThan(0m).WithMessage("El monto debe ser mayor que cero.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(SerEstadoValido)
            .WithMessage(x => $"Estado invalido: {x.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoPago>())}.");
    }

    private static bool SerEstadoValido(string estado) =>
        Enum.TryParse<EstadoPago>(estado, ignoreCase: true, out _);
}
