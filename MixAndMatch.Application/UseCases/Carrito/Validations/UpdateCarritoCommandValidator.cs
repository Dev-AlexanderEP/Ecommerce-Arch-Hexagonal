using FluentValidation;
using MixAndMatch.Application.UseCases.Carrito.Commands;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Application.UseCases.Carrito.Validations;

public class UpdateCarritoCommandValidator : AbstractValidator<UpdateCarritoCommand>
{
    public UpdateCarritoCommandValidator()
    {
        RuleFor(x => x.CarritoId)
            .GreaterThan(0).WithMessage("El id del carrito debe ser mayor que cero.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(SerEstadoValido)
            .WithMessage(x => $"Estado invalido: {x.Estado}. Permitidos: {string.Join(", ", Enum.GetNames<EstadoCarrito>())}.");
    }

    private static bool SerEstadoValido(string estado) =>
        Enum.TryParse<EstadoCarrito>(estado, ignoreCase: true, out _);
}
