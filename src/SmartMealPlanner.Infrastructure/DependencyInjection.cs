using EFCore.NamingConventions;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartMealPlanner.Application;
using SmartMealPlanner.Application.Auth.Interfaces;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Ingredients.Interfaces;
using SmartMealPlanner.Application.InventoryAlerts.Interfaces;
using SmartMealPlanner.Application.InventoryMonitoring.Interfaces;
using SmartMealPlanner.Application.MealPlanner.Interfaces;
using SmartMealPlanner.Application.Pantry.Interfaces;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;
using SmartMealPlanner.Application.Recipes.Interfaces;
using SmartMealPlanner.Application.Users.Interfaces;
using SmartMealPlanner.Application.Grocery.Interfaces;
using SmartMealPlanner.Application.Todos.Interfaces;
using SmartMealPlanner.Infrastructure.Auth;
using SmartMealPlanner.Infrastructure.Identity;
using SmartMealPlanner.Infrastructure.Persistence;
using SmartMealPlanner.Infrastructure.Persistence.Repositories;
using SmartMealPlanner.Infrastructure.Services;

namespace SmartMealPlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention());

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");

        if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
        {
            throw new InvalidOperationException("JWT secret is not configured.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub
                };
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IHouseholdContext, HouseholdContext>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IRecipeCategoryRepository, RecipeCategoryRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IPantryRepository, PantryRepository>();
        services.AddScoped<IInventoryAlertRepository, InventoryAlertRepository>();
        services.AddScoped<IInventoryMonitoringRepository, InventoryMonitoringRepository>();
        services.AddScoped<IGroceryListRepository, GroceryListRepository>();
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<IUserAccountManager, UserAccountManager>();

        return services;
    }
}
