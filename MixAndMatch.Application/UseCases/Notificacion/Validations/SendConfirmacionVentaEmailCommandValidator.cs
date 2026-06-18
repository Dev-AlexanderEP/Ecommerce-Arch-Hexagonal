using FluentValidation;
using MixAndMatch.Application.UseCases.Notificacion.Commands;

namespace MixAndMatch.Application.UseCases.Notificacion.Validations;

public class SendConfirmacionVentaEmailCommandValidator : AbstractValidator<SendConfirmacionVentaEmailCommand>
{
    public SendConfirmacionVentaEmailCommandValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0).WithMessage("El identificador de venta no es válido.");
    }
}
