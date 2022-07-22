using Microsoft.AspNetCore.Mvc;
using pfm.Commands;
using pfm.Database.Entities;
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
    public async Task<IActionResult> getTransactions([FromQuery(Name = "transaction-kind")] TransactionKind? kind,
                                                     [FromQuery(Name = "start-date")] DateTime? startTime,
                                                     [FromQuery(Name = "end-date")] DateTime? endTime,
                                                     [FromQuery(Name = "sort-by")] string? sortBy,
                                                     [FromQuery(Name = "page")] uint page = 1,
                                                     [FromQuery(Name = "page-size")] uint pageSize = 10,
                                                     [FromQuery(Name = "sort-order")] SortOrderC order = SortOrderC.asc)
    {
        var list = await service.GetTransactions(kind, startTime, endTime, sortBy, page, pageSize, order);
        if (list is null || page == 0 || pageSize == 0)
            return BadRequest();
        return Ok(list);
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