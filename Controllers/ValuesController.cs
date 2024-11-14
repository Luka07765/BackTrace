using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jade.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet("GetWeather")]
        public IEnumerable<string> GetWeather()
        {
            return new string[] { "Sunny", "Cloudy", "Rainy" };
        }
    }
}