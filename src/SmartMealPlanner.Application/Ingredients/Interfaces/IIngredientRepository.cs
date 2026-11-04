using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Ingredients.DTOs;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.Ingredients.Interfaces;

public interface IIngredientRepository
{
    Task<PaginatedList<IngredientDto>> GetPagedAsync(
        Guid householdId,
        IngredientListQuery query,
        CancellationToken cancellationToken = default);

    Task<IngredientDto?> GetByIdAsync(
        Guid householdId,
        Guid ingredientId,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        Guid householdId,
        string normalizedName,
        Guid? excludeIngredientId = null,
        CancellationToken cancellationToken = default);

    Task<IngredientDto> CreateAsync(
        Guid householdId,
        Guid userId,
        string name,
        string? defaultUnit,
        IngredientCategory? category,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<IngredientDto?> UpdateAsync(
        Guid householdId,
        Guid ingredientId,
        string name,
        string? defaultUnit,
        IngredientCategory? category,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid householdId,
        Guid ingredientId,
        CancellationToken cancellationToken = default);
}
