using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using test.Net.Models;
using test.Net.Services;

public class HomeController : Controller
{
    private readonly WeatherService _weatherService = new WeatherService();

    [HttpGet]
    public IActionResult Index()
    {
        string city = Request.Cookies["LastCity"];
        return View(new WeatherViewModel { City = city });
    }

    [HttpPost]
    public async Task<IActionResult> Index(string city)
    {
        if (string.IsNullOrEmpty(city))
        {
            ModelState.AddModelError("", "Будь ласка, введіть назву міста.");
            return View();
        }

        try
        {
            var weather = await _weatherService.GetWeatherAsync(city);

            Response.Cookies.Append("LastCity", city, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7)
            });

            if (weather.WillRain)
            {
                string cookieName = $"Warned_{city}";
                string warnedDate = Request.Cookies[cookieName];

                if (warnedDate != DateTime.Now.ToString("yyyyMMdd"))
                {
                    Response.Cookies.Append(cookieName, DateTime.Now.ToString("yyyyMMdd"), new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(1)
                    });

                    ViewBag.RainWarning = $"Увага: сьогодні в {city} очікується дощ!";
                }
            }

            return View(weather);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }
}
