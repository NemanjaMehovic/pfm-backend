using System.Security;
using Microsoft.AspNetCore.Mvc;
using pfm.Models;
using pfm.Services;

namespace pfm.Controllers;

[ApiController]
[Route("/categories")]
public class CategoriesController : ControllerBase
{
    private ICategoryService service;

    public CategoriesController(ICategoryService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> getCategories([FromQuery(Name = "parent-id")] string? parent_id)
    {
        var list = await service.SelectAll(parent_id);
        if(list is null )
            return StatusCode(440, new { message = "Internal error" });
        return Ok(new { items = list });
    }

    [HttpPost("import")]
    [Consumes("application/csv")]
    public async Task<IActionResult> import([FromBody] IEnumerable<Category> categories)
    {
        if (categories is null)
            return BadRequest();
        var list = await service.InsertMultiple(categories);
        if (list is not null)
            return StatusCode(440, new { message = list });
        return Ok("Categories imported");
    }

    [HttpDelete]
    public async Task<IActionResult> deleteAll()
    {
        var flag = await service.DeleteAll();
        if(flag)
            return Ok();
        return StatusCode(440);
    }
}