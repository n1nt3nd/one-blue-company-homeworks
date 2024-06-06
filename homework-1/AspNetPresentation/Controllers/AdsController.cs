using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetPresentation.Controllers;

[ApiController]
[Route("ads")]
public class AdsController : ControllerBase
{
    private readonly IAdsService _adsService;

    public AdsController(IAdsService adsService)
    {
        _adsService = adsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAdsAsync([FromQuery] long productId)
    {
        return Ok(await _adsService.CalculateAsync(productId));
    }
}