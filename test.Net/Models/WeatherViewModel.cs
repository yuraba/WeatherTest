namespace test.Net.Models
{
    public class WeatherViewModel
    {
        public string City { get; set; }
        public double Temperature { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public string WeatherDescription { get; set; }
        public bool WillRain { get; set; }
    }
}
