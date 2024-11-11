using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using test.Net.Models;

namespace test.Net.Services
{
    public class WeatherService
    {
        private readonly string apiKey = "b65e64ca712e7d6cd88be2bd902fd0cc";

        public async Task<WeatherViewModel> GetWeatherAsync(string city)
        {
            var coordinates = await GetCoordinatesAsync(city);
            var weatherViewModel = new WeatherViewModel();

            using (var client = new HttpClient())
            {
                string url = $"https://api.openweathermap.org/data/3.0/onecall?lat={coordinates.lat}&lon={coordinates.lon}&units=metric&exclude=minutely,alerts&appid={apiKey}";
                var response = await client.GetStringAsync(url);
                var data = JObject.Parse(response);

                weatherViewModel.City = city;
                weatherViewModel.Temperature = double.Parse(data["current"]["temp"].ToString());
                weatherViewModel.TempMin = double.Parse(data["daily"][0]["temp"]["min"].ToString());
                weatherViewModel.TempMax = double.Parse(data["daily"][0]["temp"]["max"].ToString());
                weatherViewModel.WeatherDescription = data["current"]["weather"][0]["description"].ToString();
                weatherViewModel.WillRain = data["hourly"].Any(h => h["weather"][0]["main"].ToString().ToLower().Contains("rain"));
            }

            return weatherViewModel;
        }

        public async Task<(double lat, double lon)> GetCoordinatesAsync(string city)
        {
            using (var client = new HttpClient())
            {
                string url = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={apiKey}";
                var response = await client.GetStringAsync(url);
                var data = JArray.Parse(response);

                if (data.Count > 0)
                {
                    var firstResult = data[0];
                    double lat = double.Parse(firstResult["lat"].ToString());
                    double lon = double.Parse(firstResult["lon"].ToString());
                    return (lat, lon);
                }
                else
                {
                    throw new Exception("Місто не знайдено.");
                }
            }
        }
    }

}
