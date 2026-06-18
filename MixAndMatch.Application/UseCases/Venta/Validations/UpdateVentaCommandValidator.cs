using FluentValidation;
using MixAndMatch.Application.UseCases.Venta.Commands;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Application.UseCases.Venta.Validations;

public class UpdateVentaCommandValidator : AbstractValidator<UpdateVentaCommand>
{
    public UpdateVentaCommandValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0).WithMessage("El id de la venta debe ser mayor que cero.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(SerEstadoValido)
            .WithMessage(x => $"Estado invalido: {x.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoVenta>())}.");
    }

    private static bool SerEstadoValido(string estado) =>
        Enum.TryParse<EstadoVenta>(estado, ignoreCase: true, out _);
}
