using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Recipes.DTOs;

namespace SmartMealPlanner.Application.Recipes.Interfaces;

public interface IRecipeRepository
{
    Task<PaginatedList<RecipeDto>> GetPagedAsync(
        Guid householdId,
        RecipeListQuery query,
        CancellationToken cancellationToken = default);

    Task<RecipeDto?> GetByIdAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default);

    Task<RecipeDto> CreateAsync(
        Guid householdId,
        Guid userId,
        string name,
        string? description,
        string instructions,
        int? prepTime,
        int? cookTime,
        int servings,
        Guid? categoryId,
        CancellationToken cancellationToken = default);

    Task<RecipeDto?> UpdateAsync(
        Guid householdId,
        Guid recipeId,
        string name,
        string? description,
        string instructions,
        int? prepTime,
        int? cookTime,
        int servings,
        Guid? categoryId,
        CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default);
}
