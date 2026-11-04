using FluentValidation;
using SmartMealPlanner.Application.Todos.DTOs;
using SmartMealPlanner.Application.Todos.Helpers;

namespace SmartMealPlanner.Application.Todos.Validators;

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .NotEmpty()
            .Must(TodoEnumHelper.IsValidCategory)
            .WithMessage("Category must be one of: Meal Preparation, Shopping, Household, Personal.");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .Must(TodoEnumHelper.IsValidPriority)
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.RecurrenceType)
            .NotEmpty()
            .Must(TodoEnumHelper.IsValidRecurrenceType!)
            .WithMessage("Recurrence type must be one of: Daily, Weekly, Monthly, Yearly.")
            .When(x => x.IsRecurring);

        RuleFor(x => x.RecurrenceType)
            .Must(value => string.IsNullOrWhiteSpace(value))
            .WithMessage("Recurrence type must be omitted when the task is not recurring.")
            .When(x => !x.IsRecurring);
    }
}

public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoRequest>
{
    public UpdateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .NotEmpty()
            .Must(TodoEnumHelper.IsValidCategory)
            .WithMessage("Category must be one of: Meal Preparation, Shopping, Household, Personal.");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .Must(TodoEnumHelper.IsValidPriority)
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.RecurrenceType)
            .NotEmpty()
            .Must(TodoEnumHelper.IsValidRecurrenceType!)
            .WithMessage("Recurrence type must be one of: Daily, Weekly, Monthly, Yearly.")
            .When(x => x.IsRecurring);

        RuleFor(x => x.RecurrenceType)
            .Must(value => string.IsNullOrWhiteSpace(value))
            .WithMessage("Recurrence type must be omitted when the task is not recurring.")
            .When(x => !x.IsRecurring);
    }
}

public class TodoListQueryValidator : AbstractValidator<TodoListQuery>
{
    public TodoListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Category)
            .Must(TodoEnumHelper.IsValidCategory!)
            .WithMessage("Category must be one of: Meal Preparation, Shopping, Household, Personal.")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));

        RuleFor(x => x.Priority)
            .Must(TodoEnumHelper.IsValidPriority!)
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.")
            .When(x => !string.IsNullOrWhiteSpace(x.Priority));
    }
}

public class MarkTodoCompleteRequestValidator : AbstractValidator<MarkTodoCompleteRequest>
{
    public MarkTodoCompleteRequestValidator()
    {
        // Boolean always valid; validator exists for consistency.
    }
}
