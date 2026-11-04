using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Grocery.DTOs;
using SmartMealPlanner.Application.Grocery.Helpers;
using SmartMealPlanner.Application.Grocery.Interfaces;
using SmartMealPlanner.Application.Grocery.Models;
using SmartMealPlanner.Application.MealPlanner.Helpers;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Application.Grocery.Services;

public class GroceryListService : IGroceryListService
{
    private readonly IGroceryListRepository _groceryListRepository;
    private readonly IHouseholdContext _householdContext;

    public GroceryListService(
        IGroceryListRepository groceryListRepository,
        IHouseholdContext householdContext)
    {
        _groceryListRepository = groceryListRepository;
        _householdContext = householdContext;
    }

    public async Task<IReadOnlyList<GroceryListSummaryDto>> GetListsAsync(
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _groceryListRepository.GetListsAsync(
            _householdContext.HouseholdId,
            cancellationToken);
    }

    public async Task<GroceryListDetailDto> GetListByIdAsync(
        Guid listId,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _groceryListRepository.GetListByIdAsync(
                   _householdContext.HouseholdId,
                   listId,
                   cancellationToken)
               ?? throw new NotFoundException("Grocery list", listId);
    }

    public async Task<GroceryListDetailDto> CreateListAsync(
        CreateGroceryListRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var name = string.IsNullOrWhiteSpace(request.Name)
            ? $"Grocery List – {DateOnly.FromDateTime(DateTime.UtcNow):yyyy-MM-dd}"
            : request.Name.Trim();

        return await _groceryListRepository.CreateListAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            name,
            mealPlanId: null,
            initialItems: [],
            cancellationToken);
    }

    public async Task<GroceryListDetailDto> GenerateFromMealPlanAsync(
        GenerateGroceryListFromMealPlanRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var mealPlan = await _groceryListRepository.ResolveMealPlanAsync(
            _householdContext.HouseholdId,
            request.MealPlanId,
            request.WeekStart.HasValue
                ? MealPlannerDateHelper.GetWeekStartMonday(request.WeekStart.Value)
                : null,
            cancellationToken)
            ?? throw new NotFoundException("Meal plan");

        var ingredientLines = await _groceryListRepository.GetRecipeIngredientsForMealPlanAsync(
            mealPlan.Id,
            cancellationToken);

        var aggregated = GroceryIngredientAggregator.Aggregate(ingredientLines);

        var name = string.IsNullOrWhiteSpace(request.Name)
            ? $"Grocery List – {mealPlan.WeekStartDate:yyyy-MM-dd}"
            : request.Name.Trim();

        return await _groceryListRepository.CreateListAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            name,
            mealPlan.Id,
            aggregated,
            cancellationToken);
    }

    public async Task<GroceryItemDto> AddItemAsync(
        Guid listId,
        AddGroceryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        if (request.SourceRecipeId.HasValue
            && !await _groceryListRepository.RecipeExistsInHouseholdAsync(
                _householdContext.HouseholdId,
                request.SourceRecipeId.Value,
                cancellationToken))
        {
            throw new NotFoundException("Recipe", request.SourceRecipeId.Value);
        }

        if (!await _groceryListRepository.ListBelongsToHouseholdAsync(
                _householdContext.HouseholdId,
                listId,
                cancellationToken))
        {
            throw new NotFoundException("Grocery list", listId);
        }

        var item = new AggregatedGroceryIngredient
        {
            Name = request.Name.Trim(),
            Unit = request.Unit.Trim().ToLowerInvariant(),
            Quantity = request.Quantity,
            SourceRecipeId = request.SourceRecipeId
        };

        return await _groceryListRepository.AddOrMergeItemAsync(
            _householdContext.HouseholdId,
            listId,
            item,
            request.Note,
            request.SourceRecipeId,
            cancellationToken);
    }

    public async Task<GroceryListDetailDto> AddItemsFromReceiptAsync(
        Guid listId,
        AddGroceryItemsFromReceiptRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        if (!await _groceryListRepository.ListBelongsToHouseholdAsync(
                _householdContext.HouseholdId,
                listId,
                cancellationToken))
        {
            throw new NotFoundException("Grocery list", listId);
        }

        var incoming = request.Items.Select(i => new AggregatedGroceryIngredient
        {
            Name = i.Name.Trim(),
            Unit = i.Unit.Trim().ToLowerInvariant(),
            Quantity = i.Quantity,
            SourceRecipeId = null
        }).ToList();

        var aggregated = GroceryIngredientAggregator.Aggregate(
            incoming.Select(i => new RecipeIngredientLine
            {
                Name = i.Name,
                Unit = i.Unit,
                Quantity = i.Quantity,
                RecipeId = Guid.Empty
            }));

        return await _groceryListRepository.AddOrMergeItemsAsync(
            _householdContext.HouseholdId,
            listId,
            aggregated,
            cancellationToken);
    }

    public async Task<GroceryItemDto> MarkItemPurchasedAsync(
        Guid itemId,
        MarkGroceryItemPurchasedRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        return await _groceryListRepository.MarkItemPurchasedAsync(
                   _householdContext.HouseholdId,
                   itemId,
                   request.IsPurchased,
                   cancellationToken)
               ?? throw new NotFoundException("Grocery item", itemId);
    }

    public async Task DeleteItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        if (!await _groceryListRepository.DeleteItemAsync(
                _householdContext.HouseholdId,
                itemId,
                cancellationToken))
        {
            throw new NotFoundException("Grocery item", itemId);
        }
    }
}
