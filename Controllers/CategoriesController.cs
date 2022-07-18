using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> import([FromBody] IFormFile file)
    {
        return Ok("Test import Categories");
    }
}