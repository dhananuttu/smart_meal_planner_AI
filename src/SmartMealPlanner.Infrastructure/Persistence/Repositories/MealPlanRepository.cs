using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.MealPlanner.DTOs;
using SmartMealPlanner.Application.MealPlanner.Helpers;
using SmartMealPlanner.Application.MealPlanner.Interfaces;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class MealPlanRepository : IMealPlanRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MealPlanRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WeeklyMealPlanDto> GetOrCreateWeeklyPlanAsync(
        Guid householdId,
        Guid userId,
        DateOnly weekStartDate,
        CancellationToken cancellationToken = default)
    {
        var plan = await GetOrCreatePlanEntityAsync(householdId, userId, weekStartDate, cancellationToken);
        var entries = await _dbContext.MealPlanEntries
            .AsNoTracking()
            .Include(e => e.Recipe)
            .Where(e => e.MealPlanId == plan.Id)
            .OrderBy(e => e.EntryDate)
            .ThenBy(e => e.MealSlot)
            .ThenBy(e => e.SortOrder)
            .ToListAsync(cancellationToken);

        return BuildWeeklyPlan(plan, entries);
    }

    public async Task<MealDto?> GetMealByIdAsync(
        Guid householdId,
        Guid mealId,
        CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.MealPlanEntries
            .AsNoTracking()
            .Include(e => e.Recipe)
            .Include(e => e.MealPlan)
            .FirstOrDefaultAsync(
                e => e.Id == mealId && e.MealPlan.HouseholdId == householdId,
                cancellationToken);

        return entry is null ? null : MapToMealDto(entry);
    }

    public Task<bool> RecipeExistsInHouseholdAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Recipes.AnyAsync(
            r => r.HouseholdId == householdId && r.Id == recipeId,
            cancellationToken);

    public async Task<MealDto> CreateMealAsync(
        Guid householdId,
        Guid userId,
        DateOnly date,
        string mealType,
        Guid? recipeId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var weekStart = MealPlannerDateHelper.GetWeekStartMonday(date);
        var plan = await GetOrCreatePlanEntityAsync(householdId, userId, weekStart, cancellationToken);
        var mealSlot = Enum.Parse<MealSlot>(mealType);

        var sortOrder = await _dbContext.MealPlanEntries
            .Where(e => e.MealPlanId == plan.Id && e.EntryDate == date && e.MealSlot == mealSlot)
            .MaxAsync(e => (int?)e.SortOrder, cancellationToken) ?? -1;

        var entry = new MealPlanEntry
        {
            Id = Guid.NewGuid(),
            MealPlanId = plan.Id,
            EntryDate = date,
            MealSlot = mealSlot,
            RecipeId = recipeId,
            Note = notes,
            SortOrder = sortOrder + 1
        };

        _dbContext.MealPlanEntries.Add(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (recipeId.HasValue)
        {
            await _dbContext.Entry(entry).Reference(e => e.Recipe).LoadAsync(cancellationToken);
        }

        return MapToMealDto(entry);
    }

    public async Task<MealDto?> UpdateMealAsync(
        Guid householdId,
        Guid userId,
        Guid mealId,
        DateOnly date,
        string mealType,
        Guid? recipeId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.MealPlanEntries
            .Include(e => e.MealPlan)
            .Include(e => e.Recipe)
            .FirstOrDefaultAsync(
                e => e.Id == mealId && e.MealPlan.HouseholdId == householdId,
                cancellationToken);

        if (entry is null)
        {
            return null;
        }

        var weekStart = MealPlannerDateHelper.GetWeekStartMonday(date);
        var targetPlan = entry.MealPlan.WeekStartDate == weekStart
            ? entry.MealPlan
            : await GetOrCreatePlanEntityAsync(householdId, userId, weekStart, cancellationToken);

        var mealSlot = Enum.Parse<MealSlot>(mealType);

        entry.MealPlanId = targetPlan.Id;
        entry.EntryDate = date;
        entry.MealSlot = mealSlot;
        entry.RecipeId = recipeId;
        entry.Note = notes;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (recipeId.HasValue && entry.Recipe is null)
        {
            await _dbContext.Entry(entry).Reference(e => e.Recipe).LoadAsync(cancellationToken);
        }

        return MapToMealDto(entry);
    }

    public async Task<bool> DeleteMealAsync(
        Guid householdId,
        Guid mealId,
        CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.MealPlanEntries
            .Include(e => e.MealPlan)
            .FirstOrDefaultAsync(
                e => e.Id == mealId && e.MealPlan.HouseholdId == householdId,
                cancellationToken);

        if (entry is null)
        {
            return false;
        }

        _dbContext.MealPlanEntries.Remove(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<MealPlan> GetOrCreatePlanEntityAsync(
        Guid householdId,
        Guid userId,
        DateOnly weekStartDate,
        CancellationToken cancellationToken)
    {
        var plan = await _dbContext.MealPlans
            .FirstOrDefaultAsync(
                p => p.HouseholdId == householdId && p.WeekStartDate == weekStartDate,
                cancellationToken);

        if (plan is not null)
        {
            return plan;
        }

        plan = new MealPlan
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            CreatedByUserId = userId,
            WeekStartDate = weekStartDate
        };

        _dbContext.MealPlans.Add(plan);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return plan;
    }

    private static WeeklyMealPlanDto BuildWeeklyPlan(MealPlan plan, IReadOnlyList<MealPlanEntry> entries)
    {
        var weekEnd = MealPlannerDateHelper.GetWeekEndSunday(plan.WeekStartDate);
        var mealsByDate = entries
            .GroupBy(e => e.EntryDate)
            .ToDictionary(g => g.Key, g => g.Select(MapToMealDto).ToList());

        var days = new List<DailyMealsDto>();
        for (var date = plan.WeekStartDate; date <= weekEnd; date = date.AddDays(1))
        {
            days.Add(new DailyMealsDto
            {
                Date = date,
                Meals = mealsByDate.TryGetValue(date, out var meals) ? meals : []
            });
        }

        return new WeeklyMealPlanDto
        {
            PlanId = plan.Id,
            WeekStartDate = plan.WeekStartDate,
            WeekEndDate = weekEnd,
            Days = days
        };
    }

    private static MealDto MapToMealDto(MealPlanEntry entry) =>
        new()
        {
            Id = entry.Id,
            MealPlanId = entry.MealPlanId,
            Date = entry.EntryDate,
            MealType = MealTypeHelper.ToApiValue(entry.MealSlot),
            RecipeId = entry.RecipeId,
            Recipe = entry.RecipeId.HasValue
                ? new MealRecipeSummaryDto
                {
                    Id = entry.RecipeId.Value,
                    Name = entry.Recipe?.Title ?? "Recipe unavailable",
                    IsAvailable = entry.Recipe is not null
                }
                : null,
            Notes = entry.Note,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };
}
