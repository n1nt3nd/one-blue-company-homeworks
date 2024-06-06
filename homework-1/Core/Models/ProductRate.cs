using Core.Exceptions;

namespace Core.Models;

public record ProductRate
{
    public long Id { get; init; }
    public int Month { get; init; }
    public decimal Rate { get; init; }

    public ProductRate(long id, int month, decimal rate)
    {
        if (month is < 1 or > 12)
            throw new IncorrectDataException("Month must take values from 1 to 12");
        
        if (rate is < 0 or > 3)
            throw new IncorrectDataException("Rate must take values from 0 to 3");
        
        
        Id = id;
        Month = month;
        Rate = rate;
    }
}