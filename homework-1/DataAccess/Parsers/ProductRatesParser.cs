using System.Globalization;
using Core.Exceptions;
using Core.Models;

namespace DataAccess.Parsers;

public class ProductRatesParser : IProductRatesParser
{
    public async Task<IEnumerable<ProductRate>> ParseAsync(string filePath)
    {
        var reader = new StreamReader(filePath);

        var productRates = new List<ProductRate>();
        while (!reader.EndOfStream)
        {
            string s = await reader.ReadLineAsync();
            var data = s.Split(",").ToList();
            if (data.Count != 3) throw new IncorrectDataException("Expected 3 values");
            
            if (!long.TryParse(data[0], out var id))
                throw new IncorrectDataException("Failed to parse", nameof(id));
            
            if (!int.TryParse(data[1], out var month))
                throw new IncorrectDataException("Failed to parse", nameof(month));
            
            if (!decimal.TryParse(data[2], CultureInfo.InvariantCulture, out var rate))
                throw new IncorrectDataException("Failed to parse", nameof(rate));


            productRates.Add(new ProductRate(id, month, rate));
        }
        
        reader.Close();

        return productRates;
    }
}