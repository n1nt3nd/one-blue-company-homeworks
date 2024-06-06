using Core.Exceptions;
using Core.Models;

namespace DataAccess.Parsers;

public class ProductSalesParser : IProductSalesParser
{
    public async Task<IEnumerable<ProductSale>> ParseAsync(string filePath)
    {
        var reader = new StreamReader(filePath);

        var productSales = new List<ProductSale>();
        while (!reader.EndOfStream)
        {
            string s = await reader.ReadLineAsync();
            var data = s.Split(",").ToList();
            if (data.Count != 4) throw new IncorrectDataException("Expected 4 values");
            
            if (!long.TryParse(data[0], out var id))
                throw new IncorrectDataException("Failed to parse", nameof(id));
            
            if (!DateTime.TryParse(data[1], out var date))
                throw new IncorrectDataException("Failed to parse", nameof(date));
            
            if (!int.TryParse(data[2], out var sales))
                throw new IncorrectDataException("Failed to parse", nameof(sales));


            if (!int.TryParse(data[3], out var stock))
                throw new IncorrectDataException("Failed to parse", nameof(stock));
            
            productSales.Add(new ProductSale(id, date, sales, stock));
        }
        
        reader.Close();

        return productSales;
    }
}