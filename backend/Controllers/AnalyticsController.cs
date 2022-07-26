using Microsoft.AspNetCore.Mvc;
using pfm.Database.Entities;
using pfm.Services;

namespace pfm.Controllers;

[ApiController]
[Route("/spending-analytics")]
public class AnalyticsController : ControllerBase
{

    private ITransactionService service;

    public AnalyticsController(ITransactionService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> getAnalytics([FromQuery(Name = "start-date")] DateTime? startTime,
                                                  [FromQuery(Name = "end-date")] DateTime? endTime,
                                                  [FromQuery(Name = "direction")] TransactionDirection? direction,
                                                  [FromQuery(Name = "catcode")] string? catcode)
    {
        var list = await service.GetAnalytics(direction, startTime, endTime, catcode);
        if(list is null)
            return BadRequest();
        return Ok(list);
    }
}