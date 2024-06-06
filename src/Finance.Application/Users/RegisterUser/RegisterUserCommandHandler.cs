using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Domain.Users;

namespace Finance.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IAuthenticationService authenticationService,
    IUserRepository userRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(
                new FirstName(request.FirstName),
                new LastName(request.LastName),
                new Email(request.Email),
                dateTimeProvider.UtcNow
            );

        var identityId = await authenticationService.RegisterAsync(user, request.Password, cancellationToken);

        user.SetIdentityId(identityId);
        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}
