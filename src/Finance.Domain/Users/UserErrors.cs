﻿using Finance.Domain.Abstracts;

namespace Finance.Domain.Users;

public static class UserErrors
{
    public static Error NotFound = new(
        "User.Found",
        "The user with the specified identifier was not found");

    public static Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "As credenciais informadas são inválidas");
}