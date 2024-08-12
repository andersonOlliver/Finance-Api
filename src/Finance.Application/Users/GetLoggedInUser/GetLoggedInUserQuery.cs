using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;
