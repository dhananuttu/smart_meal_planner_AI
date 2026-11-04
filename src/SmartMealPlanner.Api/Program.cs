using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SmartMealPlanner.Api.Extensions;
using SmartMealPlanner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

await app.ApplyMigrationsAsync();

app.Run();

namespace SmartMealPlanner.Api
{
    public partial class Program;
}
