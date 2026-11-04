using SmartMealPlanner.Application.Grocery.DTOs;
using SmartMealPlanner.Application.Grocery.Models;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Application.Grocery.Interfaces;

public interface IGroceryListRepository
{
    Task<IReadOnlyList<GroceryListSummaryDto>> GetListsAsync(
        Guid householdId,
        CancellationToken cancellationToken = default);

    Task<GroceryListDetailDto?> GetListByIdAsync(
        Guid householdId,
        Guid listId,
        CancellationToken cancellationToken = default);

    Task<bool> ListBelongsToHouseholdAsync(
        Guid householdId,
        Guid listId,
        CancellationToken cancellationToken = default);

    Task<GroceryListDetailDto> CreateListAsync(
        Guid householdId,
        Guid userId,
        string name,
        Guid? mealPlanId,
        IReadOnlyList<AggregatedGroceryIngredient> initialItems,
        CancellationToken cancellationToken = default);

    Task<MealPlan?> ResolveMealPlanAsync(
        Guid householdId,
        Guid? mealPlanId,
        DateOnly? weekStart,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecipeIngredientLine>> GetRecipeIngredientsForMealPlanAsync(
        Guid mealPlanId,
        CancellationToken cancellationToken = default);

    Task<bool> RecipeExistsInHouseholdAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default);

    Task<GroceryItemDto> AddOrMergeItemAsync(
        Guid householdId,
        Guid listId,
        AggregatedGroceryIngredient item,
        string? note,
        Guid? sourceRecipeId,
        CancellationToken cancellationToken = default);

    Task<GroceryListDetailDto> AddOrMergeItemsAsync(
        Guid householdId,
        Guid listId,
        IReadOnlyList<AggregatedGroceryIngredient> items,
        CancellationToken cancellationToken = default);

    Task<GroceryItemDto?> MarkItemPurchasedAsync(
        Guid householdId,
        Guid itemId,
        bool isPurchased,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteItemAsync(
        Guid householdId,
        Guid itemId,
        CancellationToken cancellationToken = default);
}
