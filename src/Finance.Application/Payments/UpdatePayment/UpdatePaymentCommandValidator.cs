using FluentValidation;

namespace Finance.Application.Payments.UpdatePayment;

internal sealed class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
{
    public UpdatePaymentCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Name).NotEmpty().MaximumLength(200).WithMessage("Informe um nome válido para a forma de pagamento");
        RuleFor(p => p.Type).IsInEnum().WithMessage("Informe um tipo de pagamento válido");
    }
}
