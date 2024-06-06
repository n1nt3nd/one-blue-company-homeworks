namespace HomeworkApp.Bll.Models;

public record RateLimiterModel
{
    public DateTimeOffset LastCheck { get; set; }

    private int _bucket;
    public int Bucket
    {
        get => _bucket;
        set => _bucket = value;
    }

    public void DecrementBucket()
    {
        Interlocked.Decrement(ref _bucket);
    }
}