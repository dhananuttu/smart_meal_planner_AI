using FluentAssertions;
using SmartMealPlanner.Application.Grocery.Helpers;
using SmartMealPlanner.Application.Grocery.Models;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class GroceryIngredientAggregatorTests
{
    [Fact]
    public void Aggregate_GroupsByNameAndUnit_CaseInsensitive()
    {
        var recipeId = Guid.NewGuid();
        var ingredients = new[]
        {
            new RecipeIngredientLine { Name = "Tomato", Unit = "kg", Quantity = 1, RecipeId = recipeId },
            new RecipeIngredientLine { Name = "tomato", Unit = "KG", Quantity = 2, RecipeId = recipeId },
            new RecipeIngredientLine { Name = "Onion", Unit = "pcs", Quantity = 3, RecipeId = recipeId }
        };

        var result = GroceryIngredientAggregator.Aggregate(ingredients);

        result.Should().HaveCount(2);
        result.Should().ContainSingle(i =>
            i.Name == "Tomato" && i.Unit == "kg" && i.Quantity == 3);
        result.Should().ContainSingle(i =>
            i.Name == "Onion" && i.Unit == "pcs" && i.Quantity == 3);
    }

    [Fact]
    public void Aggregate_ReturnsEmpty_WhenNoIngredients()
    {
        var result = GroceryIngredientAggregator.Aggregate([]);

        result.Should().BeEmpty();
    }
}
