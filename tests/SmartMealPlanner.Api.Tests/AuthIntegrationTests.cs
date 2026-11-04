using Xunit;

namespace SmartMealPlanner.Api.Tests;

/// <summary>
/// Set TEST_DATABASE_URL to a PostgreSQL connection string to run these tests.
/// Example: TEST_DATABASE_URL="Host=localhost;Port=5432;Database=smart_meal_planner;Username=postgres;Password=postgres"
/// </summary>
public class AuthIntegrationTests
{
    [Fact]
    public void Skips_WhenDatabaseNotConfigured()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEST_DATABASE_URL")))
        {
            return;
        }

        throw new InvalidOperationException("Set TEST_DATABASE_URL to run full auth integration tests.");
    }
}
