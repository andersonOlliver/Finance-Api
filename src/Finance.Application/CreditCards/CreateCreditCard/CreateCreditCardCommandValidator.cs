using FluentValidation;

namespace Finance.Application.CreditCards.CreateCreditCard;

internal sealed class CreateCreditCardCommandValidator : AbstractValidator<CreateCreditCardCommand>
{
    public CreateCreditCardCommandValidator()
    {
        RuleFor(p => p.Nickname).NotEmpty().MaximumLength(100).WithMessage("Informe um nome/apelido válido para o cartão");
        RuleFor(p => p.Brand).NotEmpty().MaximumLength(50).WithMessage("Informe a bandeira do cartão");
        RuleFor(p => p.DueDay).InclusiveBetween(1, 31).WithMessage("Informe um dia de vencimento válido (1 a 31)");
    }
}
