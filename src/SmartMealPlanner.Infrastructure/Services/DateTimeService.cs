using SmartMealPlanner.Application.Common.Interfaces;

namespace SmartMealPlanner.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public DateOnly TodayUtc => DateOnly.FromDateTime(DateTime.UtcNow);
}
