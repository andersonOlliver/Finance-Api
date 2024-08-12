using FluentValidation;

namespace Finance.Application.Users.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(p => p.FirstName).NotEmpty().WithMessage("Campo é obrigatório");
        RuleFor(p => p.LastName).NotEmpty().WithMessage("Campo é obrigatório");
        RuleFor(p => p.Email).EmailAddress().WithMessage("Informe um e-mail válido");
        RuleFor(p => p.Password).NotEmpty().MinimumLength(5).WithMessage("Informe uma senha com no mínimo 5 caracteres");
    }
}