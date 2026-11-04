using FluentValidation;
using SmartMealPlanner.Application.Recipes.DTOs;

namespace SmartMealPlanner.Application.Recipes.Validators;

public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .MaximumLength(10000);

        RuleFor(x => x.PrepTime)
            .InclusiveBetween(0, 1440)
            .When(x => x.PrepTime.HasValue);

        RuleFor(x => x.CookTime)
            .InclusiveBetween(0, 1440)
            .When(x => x.CookTime.HasValue);

        RuleFor(x => x.Servings)
            .InclusiveBetween(1, 50);
    }
}

public class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .MaximumLength(10000);

        RuleFor(x => x.PrepTime)
            .InclusiveBetween(0, 1440)
            .When(x => x.PrepTime.HasValue);

        RuleFor(x => x.CookTime)
            .InclusiveBetween(0, 1440)
            .When(x => x.CookTime.HasValue);

        RuleFor(x => x.Servings)
            .InclusiveBetween(1, 50);
    }
}

public class RecipeListQueryValidator : AbstractValidator<RecipeListQuery>
{
    public RecipeListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search));
    }
}
