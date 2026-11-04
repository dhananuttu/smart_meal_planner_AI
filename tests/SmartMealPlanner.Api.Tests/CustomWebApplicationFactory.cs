using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SmartMealPlanner.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ApplyMigrationsOnStartup"] = "false",
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Port=5432;Database=smart_meal_planner_test;Username=postgres;Password=postgres",
                ["Jwt:Secret"] = "TestOnly_SmartMealPlanner_JwtSecret_Key_Min32Chars!"
            });
        });

        builder.UseEnvironment("Development");
    }
}
