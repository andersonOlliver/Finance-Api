﻿using Finance.Application.Abstractions.Messaging;

namespace Finance.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password) : ICommand<Guid>;
