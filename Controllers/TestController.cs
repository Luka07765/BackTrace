using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private static int _currentNumber = 0;


    [HttpGet("GetCurrentNumber")]
    public IActionResult GetCurrentNumber()
    {
        return Ok(new { currentNumber = _currentNumber });
    }
}
