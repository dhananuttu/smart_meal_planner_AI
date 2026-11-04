using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Pantry.DTOs;

namespace SmartMealPlanner.Application.Pantry.Interfaces;

public interface IPantryService
{
    Task<PaginatedList<PantryItemDto>> GetInventoryAsync(
        PantryInventoryQuery query,
        CancellationToken cancellationToken = default);

    Task<PantryItemDto> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default);

    Task<PantryItemDto> AddItemAsync(
        CreatePantryItemRequest request,
        CancellationToken cancellationToken = default);

    Task<PantryItemDto> UpdateItemAsync(
        Guid itemId,
        UpdatePantryItemRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteItemAsync(Guid itemId, CancellationToken cancellationToken = default);
}
