using FluentValidation;
using MixAndMatch.Application.UseCases.Resenias.Commands;
using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Application.UseCases.Resenias.Validations;

public class UpdateEstadoReseniaCommandValidator : AbstractValidator<UpdateEstadoReseniaCommand>
{
    public UpdateEstadoReseniaCommandValidator()
    {
        RuleFor(x => x.ReseniaId)
            .GreaterThan(0).WithMessage("El id de la resenia debe ser mayor que cero.");

        RuleFor(x => x.Estado)
            .NotEqual(EstadoResenia.PENDIENTE).WithMessage("El estado de moderacion no puede ser PENDIENTE.");

        RuleFor(x => x.MotivoRechazo)
            .NotEmpty().WithMessage("El motivo de rechazo es obligatorio.")
            .MaximumLength(255).WithMessage("El motivo de rechazo no puede superar 255 caracteres.")
            .When(x => x.Estado == EstadoResenia.RECHAZADA);
    }
}
