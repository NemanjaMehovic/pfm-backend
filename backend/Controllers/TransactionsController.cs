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
        if (page == 0 || pageSize == 0)
            return BadRequest();
        var list = await service.GetTransactions(kind, startTime, endTime, sortBy, page, pageSize, order);
        if (list is null)
            return StatusCode(440, new { message = "Internal error" });
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

    [HttpDelete]
    public async Task<IActionResult> deleteAll()
    {
        var flag = await service.DeleteAll();
        if(flag)
            return Ok();
        return StatusCode(440);
    }

    [HttpPost("auto-categorize")]
    public async Task<IActionResult> auto_categorize()
    {
        var flag = await service.AutoCategorize();
        if(flag)
            return Ok("Transaction auto categorized");
        return StatusCode(440);
    }
}