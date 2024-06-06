using HomeworkApp.Utils.Helpers;

namespace HomeworkApp.Dal.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}