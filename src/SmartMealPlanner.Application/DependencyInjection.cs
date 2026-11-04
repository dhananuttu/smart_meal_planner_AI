using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartMealPlanner.Application.Auth.Interfaces;
using SmartMealPlanner.Application.Auth.Services;
using SmartMealPlanner.Application.Users.Interfaces;
using SmartMealPlanner.Application.Users.Services;
using SmartMealPlanner.Application.Ingredients.Interfaces;
using SmartMealPlanner.Application.Ingredients.Services;
using SmartMealPlanner.Application.InventoryAlerts.Interfaces;
using SmartMealPlanner.Application.InventoryAlerts.Services;
using SmartMealPlanner.Application.InventoryMonitoring.Interfaces;
using SmartMealPlanner.Application.InventoryMonitoring.Services;
using SmartMealPlanner.Application.MealPlanner.Interfaces;
using SmartMealPlanner.Application.MealPlanner.Services;
using SmartMealPlanner.Application.Pantry.Interfaces;
using SmartMealPlanner.Application.Pantry.Services;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;
using SmartMealPlanner.Application.RecipeCategories.Services;
using SmartMealPlanner.Application.Recipes.Interfaces;
using SmartMealPlanner.Application.Recipes.Services;
using SmartMealPlanner.Application.Grocery.Interfaces;
using SmartMealPlanner.Application.Grocery.Services;
using SmartMealPlanner.Application.Todos.Interfaces;
using SmartMealPlanner.Application.Todos.Services;

namespace SmartMealPlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IRecipeCategoryService, RecipeCategoryService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IMealPlanService, MealPlanService>();
        services.AddScoped<IPantryService, PantryService>();
        services.AddScoped<IInventoryAlertService, InventoryAlertService>();
        services.AddScoped<IInventoryMonitoringService, InventoryMonitoringService>();
        services.AddScoped<IGroceryListService, GroceryListService>();
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}
