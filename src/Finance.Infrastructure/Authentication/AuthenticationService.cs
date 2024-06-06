using Finance.Application.Abstractions.Authentication;
using Finance.Domain.Users;

namespace Finance.Infrastructure.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    public async Task<string> RegisterAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        return "fake-result";
    }
}
