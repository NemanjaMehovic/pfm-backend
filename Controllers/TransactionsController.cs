using System.Text.Json;
using System.Text.Json.Nodes;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc;
using pfm.Commands;
using pfm.Models;
using pfm.Services;

namespace pfm.Controllers;

[ApiController]
[Route("/transactions")]
public class TransactionsController : ControllerBase
{

    private ITransactionService service;

    public TransactionsController(ITransactionService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> getTransactions()
    {
        return Ok("Test Uspesan tranzakcije");
    }

    [HttpPost("import")]
    [Consumes("application/csv")]
    public async Task<IActionResult> import([FromBody] IEnumerable<Transaction> transactions)
    {
        if (transactions is null)
            return BadRequest();
        var list = await service.InsertMultiple(transactions);
        if (list is not null)
            return StatusCode(440, new { message = list });
        return Ok("Transaction imported");
    }

    [HttpPost("{id}/split")]
    [Consumes("application/json")]
    public async Task<IActionResult> split([FromRoute] string id, [FromBody] SplitCommand splitsCommand)
    {
        var list = await service.Split(id, splitsCommand.splits);
        if (list is not null)
            return StatusCode(440, new { message = list });
        return Ok("Transaction splitted");
    }

    [HttpPost("{id}/categorize")]
    [Consumes("application/json")]
    public async Task<IActionResult> categorize([FromRoute] string id, [FromBody] CategorizeCommand categorizeCommand)
    {
        var list = await service.Categorize(id, categorizeCommand.catcode);
        if (list is not null)
            return StatusCode(440, new { message = list });
        return Ok("Transaction categorized");
    }

    [HttpPost("auto-categorize")]
    public async Task<IActionResult> auto_categorize()
    {
        return Ok("auto-categorize");
    }
}