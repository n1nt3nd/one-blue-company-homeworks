using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetPresentation.Controllers;

[ApiController]
[Route("salesPrediction")]
public class SalesPredictionController : ControllerBase
{
    private readonly ISalesPredictionService _salesPredictionService;

    public SalesPredictionController(ISalesPredictionService salesPredictionService)
    {
        _salesPredictionService = salesPredictionService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSalesPredictionAsync([FromQuery] long productId, [FromQuery] int daysAmount)
    {
        return Ok(await _salesPredictionService.CalculateAsync(productId, daysAmount));
    }
}