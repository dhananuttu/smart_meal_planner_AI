using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Recipes.DTOs;

namespace SmartMealPlanner.Application.Recipes.Interfaces;

public interface IRecipeService
{
    Task<PaginatedList<RecipeDto>> GetAllAsync(
        RecipeListQuery query,
        CancellationToken cancellationToken = default);

    Task<RecipeDto> GetByIdAsync(Guid recipeId, CancellationToken cancellationToken = default);

    Task<RecipeDto> CreateAsync(
        CreateRecipeRequest request,
        CancellationToken cancellationToken = default);

    Task<RecipeDto> UpdateAsync(
        Guid recipeId,
        UpdateRecipeRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid recipeId, CancellationToken cancellationToken = default);
}
