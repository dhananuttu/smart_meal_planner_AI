using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Ingredients.DTOs;

namespace SmartMealPlanner.Application.Ingredients.Interfaces;

public interface IIngredientService
{
    Task<PaginatedList<IngredientDto>> GetAllAsync(
        IngredientListQuery query,
        CancellationToken cancellationToken = default);

    Task<IngredientDto> GetByIdAsync(Guid ingredientId, CancellationToken cancellationToken = default);

    Task<IngredientDto> CreateAsync(
        CreateIngredientRequest request,
        CancellationToken cancellationToken = default);

    Task<IngredientDto> UpdateAsync(
        Guid ingredientId,
        UpdateIngredientRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid ingredientId, CancellationToken cancellationToken = default);
}
