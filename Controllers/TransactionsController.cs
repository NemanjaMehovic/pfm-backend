using Microsoft.AspNetCore.Mvc;
using pfm.Models;

namespace pfm.Controllers;

[ApiController]
[Route("/transactions")]
public class TransactionsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> getTransactions()
    {
        return Ok("Test Uspesan tranzakcije");
    }

    [HttpPost("import")]
    [Consumes("application/csv")]
    public async Task<IActionResult> import([FromBody] IEnumerable<Transaction> transactions)
    {
        if(transactions is null)
            return BadRequest();
        return Ok("Import transaction");
    }

    [HttpPost("{id}/split")]
    public async Task<IActionResult> split([FromRoute] string id)
    {
        return Ok(id + " split");
    }

    [HttpPost("{id}/categorize")]
    public async Task<IActionResult> categorize([FromRoute] string id)
    {
        return Ok(id + " categorize");
    }

    [HttpPost("auto-categorize")]
    public async Task<IActionResult> auto_categorize()
    {
        return Ok("auto-categorize");
    }
}