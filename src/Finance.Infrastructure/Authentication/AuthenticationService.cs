using Finance.Application.Abstractions.Authentication;
using Finance.Domain.Users;
using Finance.Infrastructure.Authentication.Models;
using System.Net.Http.Json;

namespace Finance.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private const string PasswordCredentialType = "password";
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RegisterAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(user);
        userRepresentationModel.Credentials =
        [
           new()
           {
               Value = password,
               Temporary = false,
               Type = PasswordCredentialType
           }
        ];

        var response = await _httpClient.PostAsJsonAsync("users", userRepresentationModel, cancellationToken);

        return ExtractIdentityIdFromLocationHeader(response);
    }

    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage response)
    {
        const string usersSegmetName = "users/";
        var locationHeader = (response.Headers.Location?.PathAndQuery) ?? throw new InvalidOperationException("Location header can't be null");
        var userSegmentValueIndex = locationHeader.IndexOf(usersSegmetName, StringComparison.InvariantCultureIgnoreCase);

        var userIdentityId = locationHeader.Substring(userSegmentValueIndex + usersSegmetName.Length);
        return userIdentityId;
    }
}
