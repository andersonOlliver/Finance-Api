using Finance.Application.Abstractions.Authentication;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestUserContextAccessor
{
    public Guid UserId { get; set; } = Guid.NewGuid();
}

internal sealed class TestUserContext(TestUserContextAccessor accessor) : IUserContext
{
    public Guid UserId => accessor.UserId;

    public string IdentityId => "test-identity";
}
