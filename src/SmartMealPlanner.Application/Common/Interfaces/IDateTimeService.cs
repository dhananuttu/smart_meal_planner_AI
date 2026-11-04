namespace SmartMealPlanner.Application.Common.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }

    DateOnly TodayUtc { get; }
}
