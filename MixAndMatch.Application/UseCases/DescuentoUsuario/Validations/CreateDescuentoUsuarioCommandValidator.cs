using FluentValidation;
using MixAndMatch.Application.UseCases.DescuentoUsuario.Commands;

namespace MixAndMatch.Application.UseCases.DescuentoUsuario.Validations;

public class CreateDescuentoUsuarioCommandValidator : AbstractValidator<CreateDescuentoUsuarioCommand>
{
    public CreateDescuentoUsuarioCommandValidator()
    {
        RuleFor(x => x.DescuentoCodigoId)
            .GreaterThan(0).WithMessage("El id del código de descuento debe ser mayor que cero.");
    }
}
