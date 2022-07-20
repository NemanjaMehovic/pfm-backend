using Microsoft.AspNetCore.Mvc;
using pfm.Models;

namespace pfm.Controllers;

[ApiController]
[Route("/categories")]
public class CategoriesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> getCategories()
    {
        return Ok("Test Uspesan kategorije");
    }

    [HttpPost("import")]
    [Consumes("application/csv")]
    public async Task<IActionResult> import([FromBody] IEnumerable<Category> categories)
    {
        if(categories is null)
            return BadRequest();
        return Ok("Test import Categories");
    }
}