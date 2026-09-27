using FluentValidation;

namespace Finance.Application.Payments.CreatePayment;

internal sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MaximumLength(200).WithMessage("Informe um nome válido para a forma de pagamento");
        RuleFor(p => p.Type).IsInEnum().WithMessage("Informe um tipo de pagamento válido");
    }
}
