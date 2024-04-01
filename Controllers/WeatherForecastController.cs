using Microsoft.AspNetCore.Mvc;
using SimpleCRUD.Data;
using SimpleCRUD.Services;
using SimpleCRUD.Model;

namespace SimpleCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : Controller
    {
        public WeatherForecastController() {
        }

        [HttpPost]
        [Route("GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForecast([FromForm] Location location)
        {
            // Simulate weather forecast generation based on longitude and latitude
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

            var forecast = new
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Temperature = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)],

                Success = true,
                Message = "Weather forecast exist"
            };

            return new JsonResult(forecast);
        }

    }
}
