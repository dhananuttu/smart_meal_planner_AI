namespace SmartMealPlanner.Domain.Exceptions;

public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Invalid email or password.")
        : base(message)
    {
    }
}
