using SmartMealPlanner.Application.Grocery.DTOs;

namespace SmartMealPlanner.Application.Grocery.Interfaces;

public interface IGroceryListService
{
    Task<IReadOnlyList<GroceryListSummaryDto>> GetListsAsync(CancellationToken cancellationToken = default);

    Task<GroceryListDetailDto> GetListByIdAsync(Guid listId, CancellationToken cancellationToken = default);

    Task<GroceryListDetailDto> CreateListAsync(
        CreateGroceryListRequest request,
        CancellationToken cancellationToken = default);

    Task<GroceryListDetailDto> GenerateFromMealPlanAsync(
        GenerateGroceryListFromMealPlanRequest request,
        CancellationToken cancellationToken = default);

    Task<GroceryItemDto> AddItemAsync(
        Guid listId,
        AddGroceryItemRequest request,
        CancellationToken cancellationToken = default);

    Task<GroceryListDetailDto> AddItemsFromReceiptAsync(
        Guid listId,
        AddGroceryItemsFromReceiptRequest request,
        CancellationToken cancellationToken = default);

    Task<GroceryItemDto> MarkItemPurchasedAsync(
        Guid itemId,
        MarkGroceryItemPurchasedRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteItemAsync(Guid itemId, CancellationToken cancellationToken = default);
}
