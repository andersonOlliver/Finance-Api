using Finance.Domain.Transactions;
using FluentValidation;

namespace Finance.Application.Installments.CreateInstallmentPurchase;

internal sealed class CreateInstallmentPurchaseCommandValidator : AbstractValidator<CreateInstallmentPurchaseCommand>
{
    public CreateInstallmentPurchaseCommandValidator()
    {
        RuleFor(p => p.Title).NotEmpty().MaximumLength(90).WithMessage("Informe um título válido");
        RuleFor(p => p.TotalAmount).GreaterThan(0).WithMessage("O valor total deve ser maior que zero");
        RuleFor(p => p.CurrencyCode)
            .NotEmpty()
            .Must(code => Currency.All.Any(c => c.Code == code))
            .WithMessage("Informe uma moeda válida");
        RuleFor(p => p.InstallmentCount).InclusiveBetween(2, 60).WithMessage("O número de parcelas deve estar entre 2 e 60");
        RuleFor(p => p.CategoryId).NotEmpty().WithMessage("Informe a categoria do parcelamento");
        RuleFor(p => p.CreditCardId).NotEmpty().WithMessage("Informe o cartão de crédito utilizado");
    }
}
