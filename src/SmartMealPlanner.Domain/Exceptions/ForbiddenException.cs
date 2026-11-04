namespace SmartMealPlanner.Domain.Exceptions;

public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "You are not authorized to perform this action.")
        : base(message)
    {
    }
}
