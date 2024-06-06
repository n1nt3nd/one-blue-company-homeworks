namespace Core.Providers;

public class DateProvider : IDateProvider
{
    public DateTime GetCurrentDate()
    {
        return DateTime.Now;
    }
}