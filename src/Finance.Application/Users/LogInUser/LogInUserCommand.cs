using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Users.LogInUser;

public sealed record LogInUserCommand(string Email, string Password) : ICommand<AccessTokenResponse>;
