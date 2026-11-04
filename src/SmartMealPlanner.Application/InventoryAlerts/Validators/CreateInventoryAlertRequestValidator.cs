using FluentValidation;
using SmartMealPlanner.Application.InventoryAlerts.DTOs;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.InventoryAlerts.Validators;

public class CreateInventoryAlertRequestValidator : AbstractValidator<CreateInventoryAlertRequest>
{
    public CreateInventoryAlertRequestValidator()
    {
        RuleFor(x => x.PantryItemId)
            .NotEmpty();

        RuleFor(x => x.AlertType)
            .NotEmpty()
            .Must(type => Enum.TryParse<AlertType>(type.Trim(), ignoreCase: true, out _))
            .WithMessage("Alert type must be LowStock, OutOfStock, ExpiringSoon, or Expired.");
    }
}
