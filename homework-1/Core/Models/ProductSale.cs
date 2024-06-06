using Core.Exceptions;

namespace Core.Models;

public record ProductSale
{
    public long Id { get; init; }
    public DateTime Date { get; init; }
    public int Sales { get; init; }
    public int Stock { get; init; }

    public ProductSale(long id, DateTime date, int sales, int stock)
    {
        if (sales < 0)
            throw new IncorrectDataException("Sales must be greater than zero");
        
        if (stock < 0)
            throw new IncorrectDataException("Stocks must be greater than zero");
        
        Id = id;
        Date = date;
        Sales = sales;
        Stock = stock;
    }
}