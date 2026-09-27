using Finance.Domain.Transactions;
using FluentValidation;

namespace Finance.Application.Transactions.CreateTransaction;

internal sealed class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(p => p.Title).NotEmpty().MaximumLength(Title.MaxLenght).WithMessage("Informe um título válido");
        RuleFor(p => p.Amount).GreaterThan(0).WithMessage("O valor deve ser maior que zero");
        RuleFor(p => p.CurrencyCode)
            .NotEmpty()
            .Must(code => Currency.All.Any(c => c.Code == code))
            .WithMessage("Informe uma moeda válida");
        RuleFor(p => p.CategoryId).NotEmpty().WithMessage("Informe a categoria do lançamento");
        RuleFor(p => p.ReleasedOnUtc).NotEmpty().WithMessage("Informe a data do lançamento");
    }
}
