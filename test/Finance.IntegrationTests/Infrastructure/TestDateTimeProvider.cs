using Finance.Application.Abstractions.Clock;

namespace Finance.IntegrationTests.Infrastructure;

internal sealed class TestDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
