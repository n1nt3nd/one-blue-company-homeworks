using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetPresentation.Controllers;

[ApiController]
[Route("demand")]
public class DemandController : ControllerBase
{
    private readonly IDemandService _demandService;

    public DemandController(IDemandService demandService)
    {
        _demandService = demandService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDemandAsync([FromQuery] long productId, [FromQuery] int daysAmount)
    {
        return Ok(await _demandService.CalculateAsync(productId, daysAmount));
    }
}