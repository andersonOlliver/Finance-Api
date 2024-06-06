using FluentValidation;

namespace Finance.Application.Users.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(p => p.FirstName).NotEmpty();
        RuleFor(p => p.LastName).NotEmpty();
        RuleFor(p => p.Email).EmailAddress();
        RuleFor(p => p.Password).NotEmpty().MinimumLength(5);
    }
}