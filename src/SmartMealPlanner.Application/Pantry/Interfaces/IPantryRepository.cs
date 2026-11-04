using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Pantry.DTOs;

namespace SmartMealPlanner.Application.Pantry.Interfaces;

public interface IPantryRepository
{
    Task<PaginatedList<PantryItemDto>> GetInventoryAsync(
        Guid householdId,
        PantryInventoryQuery query,
        CancellationToken cancellationToken = default);

    Task<PantryItemDto?> GetByIdAsync(
        Guid householdId,
        Guid itemId,
        CancellationToken cancellationToken = default);

    Task<bool> IngredientExistsInHouseholdAsync(
        Guid householdId,
        Guid ingredientId,
        CancellationToken cancellationToken = default);

    Task<bool> IngredientAlreadyInPantryAsync(
        Guid householdId,
        Guid ingredientId,
        Guid? excludeItemId = null,
        CancellationToken cancellationToken = default);

    Task<PantryItemDto> CreateAsync(
        Guid householdId,
        Guid userId,
        Guid ingredientId,
        decimal quantity,
        string unit,
        DateOnly? expirationDate,
        decimal? lowStockThreshold,
        CancellationToken cancellationToken = default);

    Task<PantryItemDto?> UpdateAsync(
        Guid householdId,
        Guid itemId,
        decimal quantity,
        string unit,
        DateOnly? expirationDate,
        decimal? lowStockThreshold,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid householdId,
        Guid itemId,
        CancellationToken cancellationToken = default);
}
