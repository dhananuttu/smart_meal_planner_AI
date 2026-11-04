using FluentValidation;
using SmartMealPlanner.Application.MealPlanner.DTOs;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.MealPlanner.Validators;

public class CreateMealRequestValidator : AbstractValidator<CreateMealRequest>
{
    public CreateMealRequestValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.MealType)
            .NotEmpty()
            .Must(IsValidMealType)
            .WithMessage("Meal type must be Breakfast, Lunch, Dinner, or Snack.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }

    private static bool IsValidMealType(string mealType) =>
        Enum.TryParse<MealSlot>(mealType.Trim(), ignoreCase: true, out _);
}

public class UpdateMealRequestValidator : AbstractValidator<UpdateMealRequest>
{
    public UpdateMealRequestValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.MealType)
            .NotEmpty()
            .Must(IsValidMealType)
            .WithMessage("Meal type must be Breakfast, Lunch, Dinner, or Snack.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }

    private static bool IsValidMealType(string mealType) =>
        Enum.TryParse<MealSlot>(mealType.Trim(), ignoreCase: true, out _);
}
