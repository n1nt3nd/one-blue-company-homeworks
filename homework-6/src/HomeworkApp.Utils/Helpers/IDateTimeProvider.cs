namespace HomeworkApp.Utils.Helpers;

public interface IDateTimeProvider
{
    DateTimeOffset GetUtcNow();
}