using System.Runtime.InteropServices.JavaScript;
using Core.Abstractions;
using Core.Exceptions;
using Core.Providers;

namespace Core.Services;

public class SalesPredictionService : ISalesPredictionService
{
    private readonly IAdsService _adsService;
    private readonly IProductRatesRepository _productRatesRepository;
    private readonly IDateProvider _dateProvider;

    public SalesPredictionService(IAdsService adsService, 
        IProductRatesRepository productRatesRepository, 
        IDateProvider dateProvider)
    {
        _adsService = adsService;
        _productRatesRepository = productRatesRepository;
        _dateProvider = dateProvider;
    }

    public async Task<decimal> CalculateAsync(long productId, int daysAmount)
    {
        var ads = await _adsService.CalculateAsync(productId);

        var currentDate = _dateProvider.GetCurrentDate();
        decimal prediction = 0;
        
        while (daysAmount > 0)
        {
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            var remainingDays = int.Min(daysInMonth - currentDate.Day + 1, daysAmount);
            var seasonRate = await _productRatesRepository.GetProductRateAsync(productId, currentDate.Month);

            if (seasonRate is null)
                throw new IncorrectDataException("Season rate not specified");

            prediction += ads * remainingDays * seasonRate.Rate;

            currentDate = currentDate.AddDays(remainingDays);
            daysAmount -= remainingDays;
        }

        return prediction;
    }
}